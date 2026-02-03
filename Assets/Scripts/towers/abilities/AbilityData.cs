using UnityEngine;

namespace towers.abilities
{
    public abstract class AbilityData : ScriptableObject
    {
        public AbilityType AbilityType;
        public AbilityTrigger TriggerType;
        [TextArea] public string Description;
    }
}
