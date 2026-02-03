using System;

namespace towers.abilities.effects
{
    public abstract class Effect
    {
        public string EffectId { get; private set; }
        public float Duration { get; protected set; }
        public float ElapsedTime { get; protected set; }
        public bool IsExpired => ElapsedTime >= Duration;

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
