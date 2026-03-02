using infrastructure.services.gameStateService;
using UnityEngine;

namespace skills
{
    public abstract class PlayerSkillData : ScriptableObject
    {
        public PlayerSkillType SkillType;
        public string SkillName;
        public Sprite Icon;
        [TextArea] public string Description;
        public SkillTargetMode TargetMode;
        public GamePhase[] AvailableInPhases;
        public int[] GoldCostByLevel = new int[4];
        public float[] CooldownByLevel = new float[4];
    }
}
