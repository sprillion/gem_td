using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "SlowOnHit", menuName = "Abilities/SlowOnHit")]
    public class SlowOnHitAbility : AbilityData
    {
        [Header("Slow on Hit Settings")]
        [Tooltip("Slow percentage by level (0-3). Example: 20 = 20% slow")]
        public float[] SlowPercentByLevel = new float[4]; // [20, 30, 40, 50]

        [Tooltip("Duration in seconds by level (0-3)")]
        public float[] DurationByLevel = new float[4];    // [2, 2.5, 3, 3.5]

        private void OnValidate()
        {
            AbilityType = AbilityType.SlowOnHit;
            TriggerType = AbilityTrigger.OnHit;
        }
    }
}
