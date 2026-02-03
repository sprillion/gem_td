using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "PoisonDOT", menuName = "Abilities/PoisonDOT")]
    public class PoisonDOTAbility : AbilityData
    {
        [Header("Poison DOT Settings")]
        [Tooltip("Damage per tick by level (0-3)")]
        public int[] DamagePerTickByLevel = new int[4];   // [1, 2, 3, 5]

        [Tooltip("Tick interval in seconds by level (0-3)")]
        public float[] TickIntervalByLevel = new float[4]; // [0.5, 0.5, 0.5, 0.5]

        [Tooltip("Duration in seconds by level (0-3)")]
        public float[] DurationByLevel = new float[4];    // [3, 4, 5, 6]

        private void OnValidate()
        {
            AbilityType = AbilityType.PoisonDOT;
            TriggerType = AbilityTrigger.OnHit;
        }
    }
}
