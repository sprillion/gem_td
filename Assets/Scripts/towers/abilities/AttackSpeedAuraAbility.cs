using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "AttackSpeedAura", menuName = "Abilities/AttackSpeedAura")]
    public class AttackSpeedAuraAbility : AbilityData
    {
        [Header("Attack Speed Aura Settings")]
        [Tooltip("Aura radius by level (0-4)")]
        public float[] AuraRadiusByLevel = new float[5];        // [5, 6, 7, 8, 9]

        [Tooltip("Attack speed bonus by level (0-4). Raw Dota units: 20, 30, 40, 50, 60")]
        public float[] AttackSpeedMultiplierByLevel = new float[5]; // [20, 30, 40, 50, 60]

        private void OnValidate()
        {
            AbilityType = AbilityType.AttackSpeedAura;
            TriggerType = AbilityTrigger.Aura;
        }
    }
}
