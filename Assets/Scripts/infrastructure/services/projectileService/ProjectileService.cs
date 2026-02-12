using System;
using System.Collections.Generic;
using enemies;
using towers;
using towers.projectiles;
using UnityEngine;

namespace infrastructure.services.projectileService
{
    public class ProjectileService : IProjectileService
    {
        private const float ProjectileSpeed = 15f;

        private readonly Dictionary<TowerType, Material> _sphereMaterials = new Dictionary<TowerType, Material>();
        private readonly Dictionary<TowerType, Material> _trailMaterials = new Dictionary<TowerType, Material>();

        private static readonly TowerType[] TowerTypes =
        {
            TowerType.P, TowerType.Q, TowerType.D, TowerType.G,
            TowerType.E, TowerType.R, TowerType.B, TowerType.Y
        };

        public ProjectileService()
        {
            LoadMaterials();
        }

        private void LoadMaterials()
        {
            foreach (var towerType in TowerTypes)
            {
                var sphereMat = Resources.Load<Material>("Materials/Projectiles/Sphere_" + towerType);
                if (sphereMat != null)
                    _sphereMaterials[towerType] = sphereMat;

                var trailMat = Resources.Load<Material>("Materials/Projectiles/Trail_" + towerType);
                if (trailMat != null)
                    _trailMaterials[towerType] = trailMat;
            }
        }

        public void SpawnProjectile(Tower tower, Enemy target, Action onArrival)
        {
            if (tower == null || target == null || !target.IsAlive)
                return;

            var projectile = Pool.Get<Projectile>();
            projectile.transform.position = tower.transform.position + Vector3.up;

            var towerType = tower.TowerData.TowerType;
            var sphereMat = _sphereMaterials.TryGetValue(towerType, out var sm) ? sm : null;
            var trailMat = _trailMaterials.TryGetValue(towerType, out var tm) ? tm : null;

            projectile.Initialize(target, ProjectileSpeed, sphereMat, trailMat, onArrival);
        }
    }
}
