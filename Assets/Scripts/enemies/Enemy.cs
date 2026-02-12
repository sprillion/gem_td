using System;
using System.Collections.Generic;
using infrastructure.services.gameStateService;
using infrastructure.services.inputService;
using infrastructure.services.pathService;
using infrastructure.services.selectionService;
using level;
using level.builder;
using ui.healthBar;
using UnityEngine;
using Zenject;

namespace enemies
{
    public class Enemy : MonoBehaviour, IClickable
    {
        private IPathService _pathService;
        private ILevelBuilder _levelBuilder;
        private ISelectionService _selectionService;
        private IGameStateService _gameStateService;

        private EnemyData _enemyData;
        private List<Vector3> _path;
        private int _currentPoint;
        private int _currentHealth;
        private bool _isAlive = true;

        // Effect modifiers
        private float _moveSpeedModifier = 1f;
        private int _armorModifier = 0;

        // Health bar
        private HealthBar _healthBar;

        public int CurrentHealth => _currentHealth;
        public bool IsAlive => _isAlive;
        public EnemyData EnemyData => _enemyData;
        public int GoldReward => _enemyData != null ? _enemyData.GoldReward : 0;

        // Computed properties with modifiers
        public float CurrentMoveSpeed => _enemyData.MoveSpeed * _moveSpeedModifier;
        public int CurrentArmor => _enemyData.Armor + _armorModifier;

        public event Action<Enemy> OnDeath;
        public event Action OnReachedEnd;

        [Inject]
        private void Construct(IPathService pathService, ILevelBuilder levelBuilder, ISelectionService selectionService, IGameStateService gameStateService)
        {
            _pathService = pathService;
            _levelBuilder = levelBuilder;
            _selectionService = selectionService;
            _gameStateService = gameStateService;
        }

        public void Initialize(EnemyData enemyData)
        {
            _enemyData = enemyData;
            _currentHealth = enemyData.Health;
            _isAlive = true;
            SetPath();
            CreateClickCollider();
        }

        private void CreateClickCollider()
        {
            var clickTarget = new GameObject("ClickTarget");
            clickTarget.transform.SetParent(transform);
            clickTarget.transform.localPosition = Vector3.zero;
            clickTarget.layer = gameObject.layer;
            var collider = clickTarget.AddComponent<BoxCollider>();
            collider.size = new Vector3(1f, 2f, 1f);
        }

        public void OnClick()
        {
            _selectionService?.SelectEnemy(this);
        }

        public void SetHealthBar(HealthBar healthBar)
        {
            _healthBar = healthBar;
            _healthBar.Initialize(transform, _enemyData.Health);
        }

        public void TakeDamage(int damage)
        {
            if (!_isAlive) return;

            _currentHealth -= damage;

            if (_healthBar != null)
            {
                _healthBar.SetHealth(_currentHealth);
            }

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
            OnDeath?.Invoke(this);
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