using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "SplashDamage", menuName = "Abilities/SplashDamage")]
    public class SplashDamageAbility : AbilityData
    {
        [Header("Splash Damage Settings")]
        [Tooltip("Splash radius by level (0-3)")]
        public float[] SplashRadiusByLevel = new float[4];  // [3, 3.5, 4, 4.5]

        [Tooltip("Splash damage percentage by level (0-3). Example: 50 = 50% of primary damage")]
        public float[] SplashDamagePercentByLevel = new float[4]; // [50, 60, 70, 80]

        private void OnValidate()
        {
            AbilityType = AbilityType.SplashDamage;
            TriggerType = AbilityTrigger.OnAttack;
        }
    }
}
