using UnityEngine;

namespace towers.abilities
{
    [CreateAssetMenu(fileName = "IncreasedAttackSpeed", menuName = "Abilities/IncreasedAttackSpeed")]
    public class IncreasedAttackSpeedAbility : AbilityData
    {
        [Header("Increased Attack Speed Settings")]
        [Tooltip("Attack speed bonus by level (0-4). Raw Dota units: 200 = +200 AS passive")]
        public float[] AttackSpeedMultiplierByLevel = new float[5]; // [200, 200, 200, 200, 200]

        private void OnValidate()
        {
            AbilityType = AbilityType.IncreasedAttackSpeed;
            TriggerType = AbilityTrigger.Passive;
        }
    }
}
