using UnityEngine;

namespace towers.abilities
{
    public abstract class AbilityData : ScriptableObject
    {
        [Header("Visual")]
        public Sprite Icon;
        public string AbilityName;

        [Header("Configuration")]
        public AbilityType AbilityType;
        public AbilityTrigger TriggerType;
        [TextArea] public string Description;
    }
}
