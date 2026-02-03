using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "ArmorReduction", menuName = "Abilities/ArmorReduction")]
    public class ArmorReductionAbility : AbilityData
    {
        [Header("Armor Reduction Settings")]
        [Tooltip("Armor reduction by level (0-3)")]
        public int[] ArmorReductionByLevel = new int[4]; // [5, 10, 15, 20]

        [Tooltip("Duration in seconds by level (0-3)")]
        public float[] DurationByLevel = new float[4];    // [3, 3.5, 4, 4.5]

        private void OnValidate()
        {
            AbilityType = AbilityType.ArmorReduction;
            TriggerType = AbilityTrigger.OnHit;
        }
    }
}
