using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using enemies;
using infrastructure.factories.enemies;
using infrastructure.services.playerService;
using infrastructure.services.resourceProvider;
using level.builder;
using UnityEngine;
using Zenject;

namespace infrastructure.services.waveService
{
    public class WaveService : IWaveService
    {
        private readonly IEnemyFactory _enemyFactory;
        private readonly IResourceProvider _resourceProvider;
        private readonly IPlayerService _playerService;
        private ILevelBuilder _levelBuilder;

        private int _currentWaveNumber = 0;
        private HashSet<Enemy> _livingEnemies = new HashSet<Enemy>();
        private bool _waveInProgress = false;

        public int CurrentWaveNumber => _currentWaveNumber;

        public event Action OnWaveComplete;

        [Inject]
        public WaveService(IEnemyFactory enemyFactory, IResourceProvider resourceProvider, ILevelBuilder levelBuilder, IPlayerService playerService)
        {
            _enemyFactory = enemyFactory;
            _resourceProvider = resourceProvider;
            _levelBuilder = levelBuilder;
            _playerService = playerService;
        }

        public async void StartWave()
        {
            if (_waveInProgress)
            {
                Debug.LogWarning("Wave already in progress");
                return;
            }

            _currentWaveNumber++;
            _waveInProgress = true;
            _livingEnemies.Clear();

            Debug.Log($"Starting wave {_currentWaveNumber}");

            // Load wave configuration
            WaveConfigData config = LoadWaveConfig(_currentWaveNumber);
            if (config == null)
            {
                Debug.LogError($"Could not load wave config for wave {_currentWaveNumber}");
                CompleteWave();
                return;
            }

            // Load enemy data for this wave
            EnemyData enemyData = LoadEnemyData(_currentWaveNumber);
            if (enemyData == null)
            {
                Debug.LogError($"Could not load enemy data for wave {_currentWaveNumber}");
                CompleteWave();
                return;
            }

            // Get spawn position from map data
            if (_levelBuilder == null || _levelBuilder.MapData == null)
            {
                Debug.LogError("LevelBuilder or MapData not set");
                CompleteWave();
                return;
            }

            Vector2Int startPoint = _levelBuilder.MapData.Points.First();
            Vector3 spawnPosition = new Vector3(
                startPoint.x * _levelBuilder.MapData.BlockSize,
                0,
                -startPoint.y * _levelBuilder.MapData.BlockSize
            );

            // Spawn enemies at intervals
            for (int i = 0; i < config.EnemyCount; i++)
            {
                if (!_waveInProgress) break; // Wave was cancelled

                Enemy enemy = _enemyFactory.CreateEnemy(enemyData, spawnPosition, _currentWaveNumber);
                if (enemy != null)
                {
                    _livingEnemies.Add(enemy);

                    // Subscribe to enemy events
                    enemy.OnDeath += () => RegisterEnemyDeath(enemy);
                    enemy.OnReachedEnd += () => RegisterEnemyReachedEnd(enemy);

                    Debug.Log($"Spawned enemy {i + 1}/{config.EnemyCount}");
                }

                // Wait before spawning next enemy
                await UniTask.Delay(TimeSpan.FromSeconds(config.SpawnInterval));
            }

            Debug.Log($"All {config.EnemyCount} enemies spawned for wave {_currentWaveNumber}");
        }

        public void RegisterEnemyDeath(Enemy enemy)
        {
            if (_livingEnemies.Remove(enemy))
            {
                Debug.Log($"Enemy died. Remaining: {_livingEnemies.Count}");
                CheckWaveCompletion();
            }
        }

        public void RegisterEnemyReachedEnd(Enemy enemy)
        {
            if (_livingEnemies.Remove(enemy))
            {
                Debug.Log($"Enemy reached end. Remaining: {_livingEnemies.Count}");
                _playerService.LoseLife(1);
                CheckWaveCompletion();
            }
        }

        private void CheckWaveCompletion()
        {
            if (_livingEnemies.Count == 0 && _waveInProgress)
            {
                CompleteWave();
            }
        }

        private void CompleteWave()
        {
            _waveInProgress = false;
            Debug.Log($"Wave {_currentWaveNumber} complete!");

            // Award experience for completing the wave
            int xpReward = _currentWaveNumber * 10;
            _playerService.AwardExperience(xpReward);

            OnWaveComplete?.Invoke();
        }

        private WaveConfigData LoadWaveConfig(int waveNumber)
        {
            // Try to load specific wave configuration
            string path = $"ScriptableObjects/Waves/Wave_{waveNumber}";
            WaveConfigData config = _resourceProvider.Load<WaveConfigData>(path);

            return config;
        }

        private EnemyData LoadEnemyData(int waveNumber)
        {
            // Load enemy data specific to this wave number
            string path = $"ScriptableObjects/Enemies/EnemyData_{waveNumber}";
            EnemyData enemyData = _resourceProvider.Load<EnemyData>(path);
            return enemyData;
        }
    }
}
