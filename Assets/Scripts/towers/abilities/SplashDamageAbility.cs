using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "SplashDamage", menuName = "Abilities/SplashDamage")]
    public class SplashDamageAbility : AbilityData
    {
        [Header("Splash Damage Settings")]
        [Tooltip("Splash radius by level (0-4). Raw Dota units.")]
        public float[] SplashRadiusByLevel = new float[5];  // [300, 350, 400, 450, 500]

        [Tooltip("Splash damage percentage by level (0-4). Example: 30 = 30% of primary damage")]
        public float[] SplashDamagePercentByLevel = new float[5]; // [30, 40, 50, 60, 70]

        private void OnValidate()
        {
            AbilityType = AbilityType.SplashDamage;
            TriggerType = AbilityTrigger.OnAttack;
        }
    }
}
