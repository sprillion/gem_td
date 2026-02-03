using System;
using System.Collections.Generic;
using infrastructure.services.pathService;
using level.builder;
using UnityEngine;
using Zenject;

namespace enemies
{
    public class Enemy : MonoBehaviour
    {
        private IPathService _pathService;
        private ILevelBuilder _levelBuilder;

        private EnemyData _enemyData;
        private List<Vector3> _path;
        private int _currentPoint;
        private int _currentHealth;
        private bool _isAlive = true;

        // Effect modifiers
        private float _moveSpeedModifier = 1f;
        private int _armorModifier = 0;

        public int CurrentHealth => _currentHealth;
        public bool IsAlive => _isAlive;
        public EnemyData EnemyData => _enemyData;

        // Computed properties with modifiers
        public float CurrentMoveSpeed => _enemyData.MoveSpeed * _moveSpeedModifier;
        public int CurrentArmor => _enemyData.Armor + _armorModifier;

        public event Action OnDeath;
        public event Action OnReachedEnd;

        [Inject]
        private void Construct(IPathService pathService, ILevelBuilder levelBuilder)
        {
            _pathService = pathService;
            _levelBuilder = levelBuilder;
        }

        public void Initialize(EnemyData enemyData)
        {
            _enemyData = enemyData;
            _currentHealth = enemyData.Health;
            _isAlive = true;
            SetPath();
        }

        public void TakeDamage(int damage)
        {
            if (!_isAlive) return;

            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public void ModifyMoveSpeed(float newSpeed)
        {
            _moveSpeedModifier = newSpeed / _enemyData.MoveSpeed;
        }

        public void ModifyArmor(int armorChange)
        {
            _armorModifier += armorChange;
        }

        private void Die()
        {
            if (!_isAlive) return;

            _isAlive = false;
            OnDeath?.Invoke();
            Destroy(gameObject);
        }

        private void ReachedEnd()
        {
            if (!_isAlive) return;

            _isAlive = false;
            OnReachedEnd?.Invoke();
            Destroy(gameObject);
        }

        private void Update()
        {
            if (!_isAlive) return;
            if (_path == null || _path.Count == 0) return;
            if (_currentPoint >= _path.Count) return;

            Vector3 targetPosition = _path[_currentPoint];
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Rotate towards target
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    _enemyData.RotateSpeed * Time.deltaTime
                );
            }

            // Move directly towards target (not based on rotation)
            Move(direction);

            // Check if reached waypoint
            CheckPoint();
        }

        private void Move(Vector3 direction)
        {
            transform.position += direction * CurrentMoveSpeed * Time.deltaTime;
        }

        private void CheckPoint()
        {
            // Check if close enough to current waypoint
            if (Vector3.Distance(transform.position, _path[_currentPoint]) < 0.5f)
            {
                _currentPoint++;

                // Check if reached the end
                if (_currentPoint >= _path.Count)
                {
                    ReachedEnd();
                }
            }
        }

        private void SetPath()
        {
            // Get path based on enemy type (flying enemies ignore towers)
            bool ignoreTowers = _enemyData != null && _enemyData.EnemyMoveType == EnemyMoveType.Flying;
            var pathPoints = _pathService.FindPath(_levelBuilder.MapData, _levelBuilder.TowerMap, ignoreTowers);

            if (pathPoints == null || pathPoints.Count == 0)
            {
                Debug.LogError("No valid path found for enemy!");
                _path = new List<Vector3>();
                return;
            }

            _path = pathPoints.ConvertAll(point => new Vector3(point.x, 0, -point.y) * _levelBuilder.MapData.BlockSize);
        }
    }
}