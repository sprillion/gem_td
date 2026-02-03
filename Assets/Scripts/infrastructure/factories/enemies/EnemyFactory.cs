using System.Collections.Generic;
using enemies;
using infrastructure.services.resourceProvider;
using UnityEngine;
using Zenject;

namespace infrastructure.factories.enemies
{
    public class EnemyFactory : IEnemyFactory
    {
        private const string EnemyPrefabPath = "Prefabs/Enemies/Enemy";
        private const string EnemyModelPathFormat = "Prefabs/Enemies/Models/Enemy_{0}";

        private readonly DiContainer _diContainer;
        private readonly IResourceProvider _resourceProvider;

        private GameObject _enemyPrefab;
        private readonly Dictionary<int, GameObject> _enemyModels = new Dictionary<int, GameObject>();

        [Inject]
        public EnemyFactory(DiContainer diContainer, IResourceProvider resourceProvider)
        {
            _diContainer = diContainer;
            _resourceProvider = resourceProvider;
            LoadEnemyPrefab();
        }

        public Enemy CreateEnemy(EnemyData enemyData, Vector3 position, int waveNumber)
        {
            if (_enemyPrefab == null)
            {
                Debug.LogError("Enemy prefab not loaded");
                return null;
            }

            // Instantiate enemy with DI support
            Enemy enemy = _diContainer.InstantiatePrefabForComponent<Enemy>(_enemyPrefab, position, Quaternion.identity, null);

            // Initialize enemy with data
            enemy.Initialize(enemyData);

            // Add visual model based on wave number
            CreateEnemyModel(enemy, waveNumber);

            return enemy;
        }

        private void CreateEnemyModel(Enemy enemy, int waveNumber)
        {
            GameObject modelPrefab = LoadEnemyModel(waveNumber);

            if (modelPrefab == null)
            {
                Debug.LogWarning($"No model found for wave {waveNumber}, enemy will have no visual");
                return;
            }

            GameObject model = Object.Instantiate(modelPrefab, enemy.transform);
            model.transform.localPosition = Vector3.zero;
        }

        private GameObject LoadEnemyModel(int waveNumber)
        {
            // Check cache first
            if (_enemyModels.TryGetValue(waveNumber, out var enemyModel))
            {
                return enemyModel;
            }

            // Load model for this wave
            string path = string.Format(EnemyModelPathFormat, waveNumber);
            GameObject model = _resourceProvider.Load<GameObject>(path);

            if (model != null)
            {
                _enemyModels[waveNumber] = model;
                Debug.Log($"Loaded enemy model for wave {waveNumber} from {path}");
            }
            else
            {
                Debug.LogWarning($"Failed to load enemy model at {path}");
            }

            return model;
        }

        private void LoadEnemyPrefab()
        {
            _enemyPrefab = _resourceProvider.Load<GameObject>(EnemyPrefabPath);

            if (_enemyPrefab == null)
            {
                Debug.LogError($"Failed to load enemy prefab at {EnemyPrefabPath}");
            }
        }
    }
}
