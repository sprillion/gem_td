using UnityEngine;

namespace skills.data
{
    [CreateAssetMenu(fileName = "TowerBuffSkill", menuName = "Data/Skills/TowerBuff")]
    public class TowerBuffSkillData : PlayerSkillData
    {
        [Header("Buff Settings")]
        public float[] BuffValueByLevel = new float[4];
        public float[] BuffDurationByLevel = new float[4];
    }
}
