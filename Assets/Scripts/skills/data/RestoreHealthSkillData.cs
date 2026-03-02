using UnityEngine;

namespace skills.data
{
    [CreateAssetMenu(fileName = "RestoreHealthSkill", menuName = "Data/Skills/RestoreHealth")]
    public class RestoreHealthSkillData : PlayerSkillData
    {
        [Header("Heal Settings")]
        public int[] MinHealByLevel = new int[4];
        public int[] MaxHealByLevel = new int[4];
    }
}
