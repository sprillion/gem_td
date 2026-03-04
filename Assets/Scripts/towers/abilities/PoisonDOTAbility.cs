using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "PoisonDOT", menuName = "Abilities/PoisonDOT")]
    public class PoisonDOTAbility : AbilityData
    {
        [Header("Poison DOT Settings")]
        [Tooltip("Damage per tick by level (0-4)")]
        public int[] DamagePerTickByLevel = new int[5];   // [1, 2, 4, 8, 16]

        [Tooltip("Tick interval in seconds by level (0-4)")]
        public float[] TickIntervalByLevel = new float[5]; // [0.5, 0.5, 0.5, 0.5, 0.5]

        [Tooltip("Duration in seconds by level (0-4)")]
        public float[] DurationByLevel = new float[5];    // [5, 5, 5, 5, 5]

        private void OnValidate()
        {
            AbilityType = AbilityType.PoisonDOT;
            TriggerType = AbilityTrigger.OnHit;
        }
    }
}
