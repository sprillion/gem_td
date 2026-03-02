using towers;
using UnityEngine;

namespace skills
{
    public class TowerBuff
    {
        public Tower Tower { get; private set; }
        public PlayerSkillType SkillType { get; private set; }
        public float Value { get; private set; }
        public float Duration { get; private set; }
        public float ElapsedTime { get; private set; }
        public bool IsExpired => ElapsedTime >= Duration;

        public TowerBuff(Tower tower, PlayerSkillType skillType, float value, float duration)
        {
            Tower = tower;
            SkillType = skillType;
            Value = value;
            Duration = duration;
            ElapsedTime = 0f;
        }

        public void Apply()
        {
            switch (SkillType)
            {
                case PlayerSkillType.IncreaseRange:
                    Tower.AddRangeBuff(Value);
                    break;
                case PlayerSkillType.IncreaseAttackSpeed:
                    Tower.AddAttackSpeedBuff(Value);
                    break;
                case PlayerSkillType.IncreaseDamage:
                    Tower.AddDamageBuff(Mathf.RoundToInt(Value));
                    break;
            }
        }

        public void Remove()
        {
            switch (SkillType)
            {
                case PlayerSkillType.IncreaseRange:
                    Tower.AddRangeBuff(-Value);
                    break;
                case PlayerSkillType.IncreaseAttackSpeed:
                    Tower.AddAttackSpeedBuff(-Value);
                    break;
                case PlayerSkillType.IncreaseDamage:
                    Tower.AddDamageBuff(-Mathf.RoundToInt(Value));
                    break;
            }
        }

        public void Update(float deltaTime)
        {
            ElapsedTime += deltaTime;
        }
    }
}
