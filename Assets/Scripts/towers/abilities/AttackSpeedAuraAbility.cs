using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "AttackSpeedAura", menuName = "Abilities/AttackSpeedAura")]
    public class AttackSpeedAuraAbility : AbilityData
    {
        [Header("Attack Speed Aura Settings")]
        [Tooltip("Aura radius by level (0-3)")]
        public float[] AuraRadiusByLevel = new float[4];        // [5, 6, 7, 8]

        [Tooltip("Attack speed multiplier by level (0-3). Example: 0.1 = +10% attack speed")]
        public float[] AttackSpeedMultiplierByLevel = new float[4]; // [0.1, 0.15, 0.2, 0.25]

        private void OnValidate()
        {
            AbilityType = AbilityType.AttackSpeedAura;
            TriggerType = AbilityTrigger.Aura;
        }
    }
}
