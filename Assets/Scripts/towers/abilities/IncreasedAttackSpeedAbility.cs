using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "IncreasedAttackSpeed", menuName = "Abilities/IncreasedAttackSpeed")]
    public class IncreasedAttackSpeedAbility : AbilityData
    {
        [Header("Increased Attack Speed Settings")]
        [Tooltip("Attack speed multiplier by level (0-3). Example: 0.15 = +15% attack speed")]
        public float[] AttackSpeedMultiplierByLevel = new float[4]; // [0.15, 0.2, 0.25, 0.3]

        private void OnValidate()
        {
            AbilityType = AbilityType.IncreasedAttackSpeed;
            TriggerType = AbilityTrigger.Passive;
        }
    }
}
