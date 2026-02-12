using System;
using enemies;
using towers;

namespace infrastructure.services.projectileService
{
    public interface IProjectileService
    {
        void SpawnProjectile(Tower tower, Enemy target, Action onArrival);
    }
}
