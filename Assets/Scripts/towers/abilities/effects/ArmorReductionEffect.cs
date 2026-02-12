using enemies;
using UnityEngine;

namespace towers.abilities.effects
{
    public class ArmorReductionEffect : Effect
    {
        private readonly int _armorReduction;
        private static Sprite _cachedIcon;

        public override string DisplayName => "Armor Break";
        public override string Description => $"Armor reduced by {_armorReduction}";
        public override Sprite Icon
        {
            get
            {
                if (_cachedIcon == null)
                {
                    _cachedIcon = Resources.Load<Sprite>("Icons/Effects/ArmorReduction");
                }
                return _cachedIcon;
            }
        }

        public ArmorReductionEffect(float duration, int armorReduction) : base(duration)
        {
            _armorReduction = armorReduction;
        }

        public override void Apply(object target)
        {
            if (target is Enemy enemy)
            {
                enemy.ModifyArmor(-_armorReduction);
                Debug.Log($"Applied Armor Reduction: -{_armorReduction} armor for {Duration}s to {enemy.name}");
            }
        }

        public override void Remove(object target)
        {
            if (target is Enemy enemy)
            {
                enemy.ModifyArmor(_armorReduction);
                Debug.Log($"Removed Armor Reduction from {enemy.name}");
            }
        }
    }
}
