using System.Collections.Generic;
using enemies;
using towers;

namespace infrastructure.services.abilityService
{
    public interface IAbilityService
    {
        void ExecuteAbilities(Tower tower, Enemy primaryTarget, List<Enemy> enemiesInRange);
        void RegisterAuraTower(Tower tower);
        void UnregisterAuraTower(Tower tower);
        float GetTowerAttackSpeedMultiplier(Tower tower);
    }
}
