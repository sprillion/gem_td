using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "BonusDamage", menuName = "Abilities/BonusDamage")]
    public class BonusDamageAbility : AbilityData
    {
        [Header("Bonus Damage Settings")]
        [Tooltip("Bonus damage by level (0-3)")]
        public int[] BonusDamageByLevel = new int[4]; // [5, 10, 15, 20]

        private void OnValidate()
        {
            AbilityType = AbilityType.BonusDamage;
            TriggerType = AbilityTrigger.OnAttack;
        }
    }
}
