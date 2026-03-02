using towers;
using UnityEngine;

namespace skills.data
{
    [CreateAssetMenu(fileName = "TowerTypeChanceSkill", menuName = "Data/Skills/TowerTypeChance")]
    public class TowerTypeChanceSkillData : PlayerSkillData
    {
        [Header("Chance Settings")]
        public TowerType TargetTowerType;
        public float[] BonusChanceByLevel = new float[4];
    }
}
