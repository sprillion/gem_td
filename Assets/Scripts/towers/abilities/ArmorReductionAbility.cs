using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "ArmorReduction", menuName = "Abilities/ArmorReduction")]
    public class ArmorReductionAbility : AbilityData
    {
        [Header("Armor Reduction Settings")]
        [Tooltip("Armor reduction by level (0-4)")]
        public int[] ArmorReductionByLevel = new int[5]; // [2, 4, 8, 16, 32]

        [Tooltip("Duration in seconds by level (0-4)")]
        public float[] DurationByLevel = new float[5];    // [3, 3.5, 4, 4.5, 5]

        private void OnValidate()
        {
            AbilityType = AbilityType.ArmorReduction;
            TriggerType = AbilityTrigger.OnHit;
        }
    }
}
