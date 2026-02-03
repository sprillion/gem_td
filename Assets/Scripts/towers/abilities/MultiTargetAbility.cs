using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "MultiTarget", menuName = "Abilities/MultiTarget")]
    public class MultiTargetAbility : AbilityData
    {
        [Header("Multi-Target Settings")]
        [Tooltip("Additional targets by level (0-3). Example: 1 = hits 2 enemies total")]
        public int[] AdditionalTargetsByLevel = new int[4]; // [1, 2, 3, 4]

        private void OnValidate()
        {
            AbilityType = AbilityType.MultiTarget;
            TriggerType = AbilityTrigger.OnAttack;
        }
    }
}
