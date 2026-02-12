using enemies;
using UnityEngine;

namespace towers.abilities.effects
{
    public class SlowEffect : Effect
    {
        private readonly float _slowPercent;
        private readonly float _originalSpeed;
        private float _slowedSpeed;
        private static Sprite _cachedIcon;

        public override string DisplayName => "Slow";
        public override string Description => $"Movement speed reduced by {_slowPercent:F0}%";
        public override Sprite Icon
        {
            get
            {
                if (_cachedIcon == null)
                {
                    _cachedIcon = Resources.Load<Sprite>("Icons/Effects/Slow");
                }
                return _cachedIcon;
            }
        }

        public SlowEffect(float duration, float slowPercent) : base(duration)
        {
            _slowPercent = slowPercent;
        }

        public override void Apply(object target)
        {
            if (target is Enemy enemy)
            {
                _slowedSpeed = enemy.EnemyData.MoveSpeed * (1f - _slowPercent / 100f);
                enemy.ModifyMoveSpeed(_slowedSpeed);
                Debug.Log($"Applied Slow: {_slowPercent}% for {Duration}s to {enemy.name}");
            }
        }

        public override void Remove(object target)
        {
            if (target is Enemy enemy)
            {
                enemy.ModifyMoveSpeed(enemy.EnemyData.MoveSpeed);
                Debug.Log($"Removed Slow from {enemy.name}");
            }
        }
    }
}
