using enemies;
using UnityEngine;

namespace towers.abilities.effects
{
    public class ArmorReductionEffect : Effect
    {
        private readonly int _armorReduction;

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
