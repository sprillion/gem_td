using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "BonusDamage", menuName = "Abilities/BonusDamage")]
    public class BonusDamageAbility : AbilityData
    {
        [Header("Bonus Damage Settings")]
        [Tooltip("Bonus damage by level (0-4)")]
        public int[] BonusDamageByLevel = new int[5]; // [0, 0, 0, 20, 40]

        private void OnValidate()
        {
            AbilityType = AbilityType.BonusDamage;
            TriggerType = AbilityTrigger.OnAttack;
        }
    }
}
