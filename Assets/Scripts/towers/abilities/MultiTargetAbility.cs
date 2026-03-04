using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "MultiTarget", menuName = "Abilities/MultiTarget")]
    public class MultiTargetAbility : AbilityData
    {
        [Header("Multi-Target Settings")]
        [Tooltip("Additional targets by level (0-4). Example: 2 = hits 3 enemies total")]
        public int[] AdditionalTargetsByLevel = new int[5]; // [2, 2, 2, 2, 2]

        private void OnValidate()
        {
            AbilityType = AbilityType.MultiTarget;
            TriggerType = AbilityTrigger.OnAttack;
        }
    }
}
