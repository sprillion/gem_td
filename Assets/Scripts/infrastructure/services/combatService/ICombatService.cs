using enemies;
using towers;

namespace infrastructure.services.combatService
{
    public interface ICombatService
    {
        bool CanTowerAttack(Tower tower, float attackSpeedMultiplier = 1f);
        void RegisterAttack(Tower tower);
        void DealDamage(Enemy enemy, int damage);
    }
}
