using System.Collections.Generic;
using System.Linq;
using enemies;
using infrastructure.services.combatService;
using infrastructure.services.effectService;
using infrastructure.services.updateService;
using towers;
using towers.abilities;
using towers.abilities.effects;
using towers.projectiles;
using UnityEngine;
using Zenject;

namespace infrastructure.services.abilityService
{
    public class AbilityService : IAbilityService
    {
        private readonly IEffectService _effectService;
        private readonly ICombatService _combatService;
        private readonly IUpdateService _updateService;

        private readonly List<Tower> _auraTowers = new List<Tower>();
        private readonly Dictionary<Tower, List<Tower>> _auraBuffCache = new Dictionary<Tower, List<Tower>>();

        [Inject]
        public AbilityService(IEffectService effectService, ICombatService combatService, IUpdateService updateService)
        {
            _effectService = effectService;
            _combatService = combatService;
            _updateService = updateService;
            _updateService.OnUpdate += ProcessAuras;
        }

        public void ExecuteAbilities(Tower tower, Enemy primaryTarget, List<Enemy> enemiesInRange)
        {
            if (tower?.TowerData?.Abilities == null || tower.TowerData.Abilities.Count == 0)
                return;

            if (primaryTarget == null || !primaryTarget.IsAlive)
                return;

            int towerLevel = Mathf.Clamp(tower.TowerData.Level, 0, 3);

            foreach (var ability in tower.TowerData.Abilities)
            {
                if (ability == null) continue;

                switch (ability.TriggerType)
                {
                    case AbilityTrigger.OnAttack:
                        ExecuteOnAttackAbility(tower, primaryTarget, enemiesInRange, ability, towerLevel);
                        break;

                    case AbilityTrigger.OnHit:
                        ExecuteOnHitAbility(primaryTarget, ability, towerLevel);
                        break;

                    // Passive and Aura are handled elsewhere
                    case AbilityTrigger.Passive:
                    case AbilityTrigger.Aura:
                        break;
                }
            }
        }

        public List<ProjectileTarget> CollectAttackTargets(Tower tower, Enemy primaryTarget, List<Enemy> enemiesInRange)
        {
            var targets = new List<ProjectileTarget>();

            if (primaryTarget == null || !primaryTarget.IsAlive)
                return targets;

            int baseDamage = tower.EffectiveDamage;
            int primaryDamage = baseDamage;
            int towerLevel = Mathf.Clamp(tower.TowerData.Level, 0, 3);

            if (tower.TowerData.Abilities != null)
            {
                foreach (var ability in tower.TowerData.Abilities)
                {
                    if (ability == null) continue;

                    // BonusDamage (D): add bonus to primary target damage
                    if (ability.TriggerType == AbilityTrigger.OnAttack && ability.AbilityType == AbilityType.BonusDamage)
                    {
                        var bonusAbility = ability as BonusDamageAbility;
                        if (bonusAbility != null)
                            primaryDamage += bonusAbility.BonusDamageByLevel[towerLevel];
                    }

                    // MultiTarget (Y): add additional targets
                    if (ability.TriggerType == AbilityTrigger.OnAttack && ability.AbilityType == AbilityType.MultiTarget)
                    {
                        var multiAbility = ability as MultiTargetAbility;
                        if (multiAbility != null)
                        {
                            int additionalTargets = multiAbility.AdditionalTargetsByLevel[towerLevel];
                            var otherEnemies = enemiesInRange
                                .Where(e => e != null && e.IsAlive && e != primaryTarget)
                                .Take(additionalTargets);

                            foreach (var enemy in otherEnemies)
                            {
                                targets.Add(new ProjectileTarget(enemy, baseDamage, false));
                            }
                        }
                    }

                    // SplashDamage (R): handled on arrival via ExecuteSplashOnArrival, not added to target list
                }
            }

            // Primary target always first
            targets.Insert(0, new ProjectileTarget(primaryTarget, primaryDamage, true));

            return targets;
        }

        public void ExecuteOnHitAbilities(Tower tower, Enemy target)
        {
            if (tower?.TowerData?.Abilities == null || tower.TowerData.Abilities.Count == 0)
                return;

            if (target == null || !target.IsAlive)
                return;

            int towerLevel = Mathf.Clamp(tower.TowerData.Level, 0, 3);

            foreach (var ability in tower.TowerData.Abilities)
            {
                if (ability == null) continue;

                if (ability.TriggerType == AbilityTrigger.OnHit)
                {
                    ExecuteOnHitAbility(target, ability, towerLevel);
                }
            }
        }

        public void ExecuteSplashOnArrival(Tower tower, Enemy primaryTarget, List<Enemy> enemiesInRange)
        {
            if (tower?.TowerData?.Abilities == null)
                return;

            if (primaryTarget == null || !primaryTarget.IsAlive)
                return;

            int towerLevel = Mathf.Clamp(tower.TowerData.Level, 0, 3);

            foreach (var ability in tower.TowerData.Abilities)
            {
                if (ability == null) continue;

                if (ability.TriggerType == AbilityTrigger.OnAttack && ability.AbilityType == AbilityType.SplashDamage)
                {
                    ExecuteSplashDamage(tower, primaryTarget, enemiesInRange, ability as SplashDamageAbility, towerLevel);
                }
            }
        }

        private void ExecuteOnAttackAbility(Tower tower, Enemy primaryTarget, List<Enemy> enemiesInRange, AbilityData ability, int level)
        {
            switch (ability.AbilityType)
            {
                case AbilityType.BonusDamage:
                    ExecuteBonusDamage(tower, primaryTarget, ability as BonusDamageAbility, level);
                    break;

                case AbilityType.SplashDamage:
                    ExecuteSplashDamage(tower, primaryTarget, enemiesInRange, ability as SplashDamageAbility, level);
                    break;

                case AbilityType.MultiTarget:
                    ExecuteMultiTarget(tower, primaryTarget, enemiesInRange, ability as MultiTargetAbility, level);
                    break;
            }
        }

        private void ExecuteOnHitAbility(Enemy target, AbilityData ability, int level)
        {
            switch (ability.AbilityType)
            {
                case AbilityType.SlowOnHit:
                    ApplySlowEffect(target, ability as SlowOnHitAbility, level);
                    break;

                case AbilityType.PoisonDOT:
                    ApplyPoisonEffect(target, ability as PoisonDOTAbility, level);
                    break;

                case AbilityType.ArmorReduction:
                    ApplyArmorReductionEffect(target, ability as ArmorReductionAbility, level);
                    break;
            }
        }

        private void ExecuteBonusDamage(Tower tower, Enemy target, BonusDamageAbility ability, int level)
        {
            if (ability == null) return;

            int bonusDamage = ability.BonusDamageByLevel[level];
            _combatService.DealDamage(target, bonusDamage);
            Debug.Log($"Tower {tower.TowerData.TowerType} dealt {bonusDamage} bonus damage to {target.name}");
        }

        private void ExecuteSplashDamage(Tower tower, Enemy primaryTarget, List<Enemy> enemiesInRange, SplashDamageAbility ability, int level)
        {
            if (ability == null) return;

            float splashRadius = ability.SplashRadiusByLevel[level];
            float splashPercent = ability.SplashDamagePercentByLevel[level];
            int splashDamage = Mathf.RoundToInt(tower.EffectiveDamage * (splashPercent / 100f));

            // Find enemies near primary target
            foreach (var enemy in enemiesInRange)
            {
                if (enemy == null || !enemy.IsAlive || enemy == primaryTarget)
                    continue;

                float distance = Vector3.Distance(primaryTarget.transform.position, enemy.transform.position);
                if (distance <= splashRadius)
                {
                    _combatService.DealDamage(enemy, splashDamage);
                    Debug.Log($"Tower {tower.TowerData.TowerType} splash damage {splashDamage} to {enemy.name}");
                }
            }
        }

        private void ExecuteMultiTarget(Tower tower, Enemy primaryTarget, List<Enemy> enemiesInRange, MultiTargetAbility ability, int level)
        {
            if (ability == null) return;

            int additionalTargets = ability.AdditionalTargetsByLevel[level];
            int totalTargets = 1 + additionalTargets; // Primary + additional

            // Get all alive enemies except primary
            var otherEnemies = enemiesInRange
                .Where(e => e != null && e.IsAlive && e != primaryTarget)
                .Take(additionalTargets)
                .ToList();

            // Deal full damage to each additional target
            foreach (var enemy in otherEnemies)
            {
                _combatService.DealDamage(enemy, tower.EffectiveDamage);
                Debug.Log($"Tower {tower.TowerData.TowerType} multi-target damage {tower.EffectiveDamage} to {enemy.name}");
            }

            Debug.Log($"Tower {tower.TowerData.TowerType} hit {otherEnemies.Count + 1}/{totalTargets} targets");
        }

        private void ApplySlowEffect(Enemy target, SlowOnHitAbility ability, int level)
        {
            if (ability == null) return;

            float slowPercent = ability.SlowPercentByLevel[level];
            float duration = ability.DurationByLevel[level];

            var effect = new SlowEffect(duration, slowPercent);
            _effectService.ApplyEffect(target, effect);
        }

        private void ApplyPoisonEffect(Enemy target, PoisonDOTAbility ability, int level)
        {
            if (ability == null) return;

            int damagePerTick = ability.DamagePerTickByLevel[level];
            float tickInterval = ability.TickIntervalByLevel[level];
            float duration = ability.DurationByLevel[level];

            var effect = new PoisonEffect(duration, damagePerTick, tickInterval);
            _effectService.ApplyEffect(target, effect);
        }

        private void ApplyArmorReductionEffect(Enemy target, ArmorReductionAbility ability, int level)
        {
            if (ability == null) return;

            int armorReduction = ability.ArmorReductionByLevel[level];
            float duration = ability.DurationByLevel[level];

            var effect = new ArmorReductionEffect(duration, armorReduction);
            _effectService.ApplyEffect(target, effect);
        }

        public void RegisterAuraTower(Tower tower)
        {
            if (tower != null && !_auraTowers.Contains(tower))
            {
                _auraTowers.Add(tower);
                Debug.Log($"Registered aura tower: {tower.TowerData.TowerType}");
            }
        }

        public void UnregisterAuraTower(Tower tower)
        {
            if (tower != null && _auraTowers.Contains(tower))
            {
                _auraTowers.Remove(tower);
                Debug.Log($"Unregistered aura tower: {tower.TowerData.TowerType}");

                // Clear cache entries related to this tower
                _auraBuffCache.Remove(tower);
                foreach (var kvp in _auraBuffCache.ToList())
                {
                    kvp.Value.Remove(tower);
                    if (kvp.Value.Count == 0)
                    {
                        _auraBuffCache.Remove(kvp.Key);
                    }
                }
            }
        }

        public float GetTowerAttackSpeedBonus(Tower tower)
        {
            if (tower?.TowerData?.Abilities == null)
                return 0f;

            // Dota 2-style: bonuses are additive raw attack speed points
            float bonus = 0f;
            int towerLevel = Mathf.Clamp(tower.TowerData.Level, 0, 4);

            // Passive attack speed (Q tower): +200 AS added to base
            foreach (var ability in tower.TowerData.Abilities)
            {
                if (ability is IncreasedAttackSpeedAbility passiveAbility)
                {
                    bonus += passiveAbility.AttackSpeedMultiplierByLevel[towerLevel];
                }
            }

            // Aura buffs from E towers: +20/30/40/50/60 AS added to base
            if (_auraBuffCache.ContainsKey(tower))
            {
                foreach (var auraTower in _auraBuffCache[tower])
                {
                    if (auraTower == null || !auraTower.enabled) continue;

                    int auraLevel = Mathf.Clamp(auraTower.TowerData.Level, 0, 4);
                    foreach (var ability in auraTower.TowerData.Abilities)
                    {
                        if (ability is AttackSpeedAuraAbility auraAbility)
                        {
                            bonus += auraAbility.AttackSpeedMultiplierByLevel[auraLevel];
                        }
                    }
                }
            }

            return bonus;
        }

        private void ProcessAuras()
        {
            // Clear cache
            _auraBuffCache.Clear();

            // Process each aura tower
            foreach (var auraTower in _auraTowers)
            {
                if (auraTower == null || !auraTower.enabled) continue;

                int auraLevel = Mathf.Clamp(auraTower.TowerData.Level, 0, 3);

                // Find the aura ability
                AttackSpeedAuraAbility auraAbility = null;
                foreach (var ability in auraTower.TowerData.Abilities)
                {
                    if (ability is AttackSpeedAuraAbility aura)
                    {
                        auraAbility = aura;
                        break;
                    }
                }

                if (auraAbility == null) continue;

                float auraRadius = auraAbility.AuraRadiusByLevel[auraLevel];

                // Find all towers in radius using OverlapSphere
                Collider[] colliders = Physics.OverlapSphere(auraTower.transform.position, auraRadius);
                foreach (var collider in colliders)
                {
                    var tower = collider.GetComponent<Tower>();
                    if (tower != null && tower != auraTower && tower.enabled)
                    {
                        if (!_auraBuffCache.ContainsKey(tower))
                        {
                            _auraBuffCache[tower] = new List<Tower>();
                        }

                        if (!_auraBuffCache[tower].Contains(auraTower))
                        {
                            _auraBuffCache[tower].Add(auraTower);
                        }
                    }
                }
            }
        }
    }
}
