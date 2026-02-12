using System.Collections.Generic;
using enemies;
using towers;
using towers.projectiles;

namespace infrastructure.services.abilityService
{
    public interface IAbilityService
    {
        void ExecuteAbilities(Tower tower, Enemy primaryTarget, List<Enemy> enemiesInRange);
        List<ProjectileTarget> CollectAttackTargets(Tower tower, Enemy primaryTarget, List<Enemy> enemiesInRange);
        void ExecuteOnHitAbilities(Tower tower, Enemy target);
        void ExecuteSplashOnArrival(Tower tower, Enemy primaryTarget, List<Enemy> enemiesInRange);
        void RegisterAuraTower(Tower tower);
        void UnregisterAuraTower(Tower tower);
        float GetTowerAttackSpeedMultiplier(Tower tower);
    }
}
