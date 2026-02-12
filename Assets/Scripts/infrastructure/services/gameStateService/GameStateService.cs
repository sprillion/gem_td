using System;
using System.Collections.Generic;
using infrastructure.factories.towers;
using infrastructure.services.pathService;
using infrastructure.services.waveService;
using level.builder;
using towers;
using UnityEngine;
using Zenject;

namespace infrastructure.services.gameStateService
{
    public class GameStateService : IGameStateService
    {
        private readonly IPathService _pathService;
        private readonly IWaveService _waveService;
        private readonly ITowerFactory _towerFactory;
        private ILevelBuilder _levelBuilder;

        private GamePhase _currentPhase = GamePhase.PLACING_TOWERS;
        private List<Tower> _placedTowersThisRound = new List<Tower>();

        public GamePhase CurrentPhase => _currentPhase;
        public int TowersPlacedThisRound => _placedTowersThisRound.Count;

        public event Action<GamePhase> OnPhaseChanged;

        [Inject]
        public GameStateService(IPathService pathService, IWaveService waveService, ILevelBuilder levelBuilder, ITowerFactory towerFactory)
        {
            _pathService = pathService;
            _waveService = waveService;
            _levelBuilder = levelBuilder;
            _towerFactory = towerFactory;

            // Subscribe to wave completion
            _waveService.OnWaveComplete += OnWaveComplete;
        }

        public void RegisterTowerPlacement(Tower tower)
        {
            if (_currentPhase != GamePhase.PLACING_TOWERS)
            {
                Debug.LogWarning("Cannot place towers during current phase: " + _currentPhase);
                return;
            }

            _placedTowersThisRound.Add(tower);
            Debug.Log($"Tower placed ({_placedTowersThisRound.Count}/5)");

            if (_placedTowersThisRound.Count >= 5)
            {
                TransitionToPhase(GamePhase.SELECTING_TOWER);
                EnableTowerHighlights();
            }
        }

        public void SelectTower(Tower selectedTower)
        {
            if (_currentPhase != GamePhase.SELECTING_TOWER)
            {
                Debug.LogWarning("Cannot select tower during current phase: " + _currentPhase);
                return;
            }

            if (!_placedTowersThisRound.Contains(selectedTower))
            {
                Debug.LogWarning("Selected tower was not placed this round");
                return;
            }

            Debug.Log("Tower selected, converting others to stone");

            // Disable highlights first
            DisableTowerHighlights();

            // Convert all non-selected towers to stone
            foreach (var tower in _placedTowersThisRound)
            {
                if (tower != selectedTower)
                {
                    ConvertTowerToStone(tower);
                }
            }

            // Clear the list for next round
            _placedTowersThisRound.Clear();

            // Start the wave
            StartWave();
        }

        private void ConvertTowerToStone(Tower tower)
        {
            if (_levelBuilder == null)
            {
                Debug.LogError("LevelBuilder not set in GameStateService");
                return;
            }

            // Get grid position from world position
            Vector3 worldPos = tower.transform.position;
            int x = Mathf.RoundToInt(worldPos.x / _levelBuilder.MapData.BlockSize);
            int y = Mathf.RoundToInt(-worldPos.z / _levelBuilder.MapData.BlockSize);

            // Update tower map
            _levelBuilder.SetTowerType(x, y, TowerType.Stone);

            // Disable tower combat functionality
            var enemyTrigger = tower.GetComponent<EnemyTrigger>();
            if (enemyTrigger != null)
            {
                GameObject.Destroy(enemyTrigger);
            }

            // Disable tower script
            tower.enabled = false;

            // Replace visual model with stone model
            _towerFactory.ReplaceWithStoneModel(tower);

            Debug.Log($"Tower at ({x}, {y}) converted to stone");
        }

        public void StartWave()
        {
            TransitionToPhase(GamePhase.COMBAT);
            _waveService.StartWave();
        }

        public void EndWave()
        {
            Debug.Log("Wave ended, returning to tower placement");
            TransitionToPhase(GamePhase.PLACING_TOWERS);
        }

        private void OnWaveComplete()
        {
            EndWave();
        }

        private void EnableTowerHighlights()
        {
            Debug.Log("Enabling tower highlights for selection");
            foreach (var tower in _placedTowersThisRound)
            {
                if (tower != null)
                {
                    tower.SetHighlight(true);
                }
            }
        }

        private void DisableTowerHighlights()
        {
            Debug.Log("Disabling tower highlights");
            foreach (var tower in _placedTowersThisRound)
            {
                if (tower != null)
                {
                    tower.SetHighlight(false);
                }
            }
        }

        private void TransitionToPhase(GamePhase newPhase)
        {
            _currentPhase = newPhase;
            Debug.Log("Phase changed to: " + newPhase);
            OnPhaseChanged?.Invoke(newPhase);
        }
    }
}
