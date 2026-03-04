using System.Collections.Generic;
using System.Linq;
using enemies;
using infrastructure.services.combatService;
using infrastructure.services.updateService;
using towers.abilities.effects;
using UnityEngine;
using Zenject;

namespace infrastructure.services.effectService
{
    public class EffectService : IEffectService
    {
        private readonly IUpdateService _updateService;
        private readonly ICombatService _combatService;
        private readonly Dictionary<Enemy, List<Effect>> _activeEffects = new Dictionary<Enemy, List<Effect>>();

        [Inject]
        public EffectService(IUpdateService updateService, ICombatService combatService)
        {
            _updateService = updateService;
            _combatService = combatService;
            _updateService.OnUpdate += OnUpdate;
        }

        public void ApplyEffect(Enemy enemy, Effect effect)
        {
            if (enemy == null || !enemy.IsAlive)
                return;

            if (!_activeEffects.ContainsKey(enemy))
            {
                _activeEffects[enemy] = new List<Effect>();
                enemy.OnDeath += ClearEffects;
            }

            var existingEffect = _activeEffects[enemy].FirstOrDefault(e => e.GetType() == effect.GetType());
            if (existingEffect != null)
            {
                existingEffect.Remove(enemy);
                _activeEffects[enemy].Remove(existingEffect);
                Debug.Log($"Refreshed effect {effect.GetType().Name} on {enemy.name}");
            }

            _activeEffects[enemy].Add(effect);
            effect.Apply(enemy);
        }

        public void RemoveEffect(Enemy enemy, string effectId)
        {
            if (!_activeEffects.ContainsKey(enemy))
                return;

            var effect = _activeEffects[enemy].FirstOrDefault(e => e.EffectId == effectId);
            if (effect != null)
            {
                effect.Remove(enemy);
                _activeEffects[enemy].Remove(effect);

                if (_activeEffects[enemy].Count == 0)
                {
                    _activeEffects.Remove(enemy);
                }
            }
        }

        public void ClearEffects(Enemy enemy)
        {
            if (!_activeEffects.ContainsKey(enemy))
                return;

            foreach (var effect in _activeEffects[enemy])
            {
                effect.Remove(enemy);
            }

            _activeEffects.Remove(enemy);
            Debug.Log($"Cleared all effects from {enemy.name}");
        }

        public List<Effect> GetActiveEffects(Enemy enemy)
        {
            if (!_activeEffects.ContainsKey(enemy))
                return new List<Effect>();

            return new List<Effect>(_activeEffects[enemy]);
        }

        private void OnUpdate()
        {
            var enemiesToRemove = new List<Enemy>();
            var effectsToRemove = new List<(Enemy enemy, Effect effect)>();

            foreach (var kvp in _activeEffects)
            {
                var enemy = kvp.Key;
                var effects = kvp.Value;

                if (enemy == null || !enemy.IsAlive)
                {
                    enemiesToRemove.Add(enemy);
                    continue;
                }

                foreach (var effect in effects)
                {
                    effect.Update(Time.deltaTime);

                    if (effect is PoisonEffect poisonEffect)
                    {
                        if (poisonEffect.ShouldDealDamage(Time.deltaTime))
                        {
                            _combatService.DealDamage(enemy, poisonEffect.GetDamage());
                            Debug.Log($"Poison dealt {poisonEffect.GetDamage()} damage to {enemy.name}");
                        }
                    }

                    if (effect.IsExpired)
                    {
                        effectsToRemove.Add((enemy, effect));
                    }
                }
            }

            foreach (var (enemy, effect) in effectsToRemove)
            {
                effect.Remove(enemy);
                _activeEffects[enemy].Remove(effect);

                if (_activeEffects[enemy].Count == 0)
                {
                    _activeEffects.Remove(enemy);
                }
            }

            foreach (var enemy in enemiesToRemove)
            {
                _activeEffects.Remove(enemy);
            }
        }
    }
}
