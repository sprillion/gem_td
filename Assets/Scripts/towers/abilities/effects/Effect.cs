using System;
using UnityEngine;

namespace towers.abilities.effects
{
    public abstract class Effect
    {
        public string EffectId { get; private set; }
        public float Duration { get; protected set; }
        public float ElapsedTime { get; protected set; }
        public bool IsExpired => ElapsedTime >= Duration;
        public float RemainingDuration => Mathf.Max(0f, Duration - ElapsedTime);

        // Display properties for UI
        public abstract string DisplayName { get; }
        public abstract string Description { get; }
        public abstract Sprite Icon { get; }

        protected Effect(float duration)
        {
            EffectId = Guid.NewGuid().ToString();
            Duration = duration;
            ElapsedTime = 0f;
        }

        public abstract void Apply(object target);
        public abstract void Remove(object target);

        public virtual void Update(float deltaTime)
        {
            ElapsedTime += deltaTime;
        }
    }
}
