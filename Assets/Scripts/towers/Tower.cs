using System.Collections.Generic;
using System.Linq;
using enemies;
using UnityEngine;

namespace towers
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] private EnemyTrigger _enemyTrigger;
        
        private readonly List<Enemy> _enemiesInside = new List<Enemy>();
        private Enemy _currentTarget;

        public TowerData TowerData { get; private set; }
        public Transform ScaledObject;

        public void Initialize(TowerData towerData)
        {
            TowerData = towerData;
            Subscribes();
        }
        
        private void Subscribes()
        {
            _enemyTrigger.OnEnemyEnter += AddEnemy;
            _enemyTrigger.OnEnemyExit += RemoveEnemy;
        }

        private void AddEnemy(Enemy enemy)
        {
            _enemiesInside.Add(enemy);
            if (_currentTarget == null)
            {
                _currentTarget = enemy;
            }
        }

        private void RemoveEnemy(Enemy enemy)
        {
            _enemiesInside.Remove(enemy);
            if (_currentTarget == enemy)
            {
                _currentTarget = _enemiesInside.FirstOrDefault();
            }
        }
    }
}