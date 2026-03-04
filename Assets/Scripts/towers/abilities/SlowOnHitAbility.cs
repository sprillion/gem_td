using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "SlowOnHit", menuName = "Abilities/SlowOnHit")]
    public class SlowOnHitAbility : AbilityData
    {
        [Header("Slow on Hit Settings")]
        [Tooltip("Slow percentage by level (0-4). Example: 60 = 60% slow")]
        public float[] SlowPercentByLevel = new float[5]; // [60, 90, 120, 150, 180]

        [Tooltip("Duration in seconds by level (0-4)")]
        public float[] DurationByLevel = new float[5];    // [2, 2.5, 3, 3.5, 4]

        private void OnValidate()
        {
            AbilityType = AbilityType.SlowOnHit;
            TriggerType = AbilityTrigger.OnHit;
        }
    }
}
