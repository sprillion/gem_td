using System.Collections.Generic;
using enemies;
using towers;
using UnityEngine;

namespace infrastructure.services.combatService
{
    public class CombatService : ICombatService
    {
        private Dictionary<Tower, float> _lastAttackTime = new Dictionary<Tower, float>();

        public bool CanTowerAttack(Tower tower, float attackSpeedBonus = 0f)
        {
            if (tower.TowerData == null) return false;

            // Dota 2 formula: cooldown = BAT * 100 / totalAS = 170 / totalAS
            // where 170 = Base Attack Time (1.7) * 100
            float totalAttackSpeed = tower.TowerData.AttackSpeed + attackSpeedBonus;
            float cooldown = 170f / totalAttackSpeed;

            if (!_lastAttackTime.ContainsKey(tower))
            {
                return true;
            }

            float timeSinceLastAttack = Time.time - _lastAttackTime[tower];
            return timeSinceLastAttack >= cooldown;
        }

        public void RegisterAttack(Tower tower)
        {
            _lastAttackTime[tower] = Time.time;
        }

        public void DealDamage(Enemy enemy, int damage)
        {
            if (enemy == null || !enemy.IsAlive)
            {
                return;
            }

            // Apply armor damage reduction
            int effectiveDamage = CalculateDamageWithArmor(damage, enemy.CurrentArmor);
            enemy.TakeDamage(effectiveDamage);
        }

        private int CalculateDamageWithArmor(int rawDamage, int armor)
        {
            // Formula: Damage = Raw × (100 / (100 + Armor))
            float damageMultiplier = 100f / (100f + Mathf.Max(0, armor));
            return Mathf.Max(1, (int)(rawDamage * damageMultiplier));
        }
    }
}
