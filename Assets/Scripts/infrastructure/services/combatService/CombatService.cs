using System.Collections.Generic;
using enemies;
using towers;
using UnityEngine;

namespace infrastructure.services.combatService
{
    public class CombatService : ICombatService
    {
        private Dictionary<Tower, float> _lastAttackTime = new Dictionary<Tower, float>();

        public bool CanTowerAttack(Tower tower, float attackSpeedMultiplier = 1f)
        {
            if (tower.TowerData == null) return false;

            float effectiveAttackSpeed = tower.TowerData.AttackSpeed * attackSpeedMultiplier;
            float cooldown = 1f / effectiveAttackSpeed;

            if (!_lastAttackTime.ContainsKey(tower))
            {
                return true;
            }

            float timeSinceLastAttack = Time.time - _lastAttackTime[tower];
            return timeSinceLastAttack >= cooldown;
        }

        public void RegisterAttack(Tower tower)
        {
            _lastAttackTime[tower] = Time.time;
        }

        public void DealDamage(Enemy enemy, int damage)
        {
            if (enemy == null || !enemy.IsAlive)
            {
                return;
            }

            // Apply armor damage reduction
            int effectiveDamage = CalculateDamageWithArmor(damage, enemy.CurrentArmor);
            enemy.TakeDamage(effectiveDamage);
        }

        private int CalculateDamageWithArmor(int rawDamage, int armor)
        {
            // Formula: Damage = Raw × (100 / (100 + Armor))
            float damageMultiplier = 100f / (100f + Mathf.Max(0, armor));
            return Mathf.Max(1, (int)(rawDamage * damageMultiplier));
        }
    }
}
