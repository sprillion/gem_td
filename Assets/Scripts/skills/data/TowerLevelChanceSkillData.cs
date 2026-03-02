using UnityEngine;

namespace skills.data
{
    [CreateAssetMenu(fileName = "TowerLevelChanceSkill", menuName = "Data/Skills/TowerLevelChance")]
    public class TowerLevelChanceSkillData : PlayerSkillData
    {
        [Header("Chance Settings")]
        public int TargetTowerLevel;
        public float[] BonusChanceByLevel = new float[4];
    }
}
