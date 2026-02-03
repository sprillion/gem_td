using enemies;
using UnityEngine;

namespace towers.abilities.effects
{
    public class PoisonEffect : Effect
    {
        private readonly int _damagePerTick;
        private readonly float _tickInterval;
        private float _timeSinceLastTick;

        public PoisonEffect(float duration, int damagePerTick, float tickInterval) : base(duration)
        {
            _damagePerTick = damagePerTick;
            _tickInterval = tickInterval;
            _timeSinceLastTick = 0f;
        }

        public override void Apply(object target)
        {
            if (target is Enemy enemy)
            {
                Debug.Log($"Applied Poison: {_damagePerTick} damage every {_tickInterval}s for {Duration}s to {enemy.name}");
            }
        }

        public override void Remove(object target)
        {
            if (target is Enemy enemy)
            {
                Debug.Log($"Removed Poison from {enemy.name}");
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            _timeSinceLastTick += deltaTime;

            if (_timeSinceLastTick >= _tickInterval)
            {
                _timeSinceLastTick -= _tickInterval;
                // Damage will be applied by EffectService
            }
        }

        public bool ShouldDealDamage(float deltaTime)
        {
            return _timeSinceLastTick >= _tickInterval;
        }

        public int GetDamage()
        {
            return _damagePerTick;
        }
    }
}
