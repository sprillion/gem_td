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

        private GamePhase _currentPhase = GamePhase.SKILL_SELECTION;
        private List<Tower> _placedTowersThisRound = new List<Tower>();

        public GamePhase CurrentPhase => _currentPhase;
        public int TowersPlacedThisRound => _placedTowersThisRound.Count;

        public event Action<GamePhase> OnPhaseChanged;

        [Inject]
        public GameStateService(IPathService pathService, IWaveService waveService, ITowerFactory towerFactory)
        {
            _pathService = pathService;
            _waveService = waveService;
            _towerFactory = towerFactory;

            _waveService.OnWaveComplete += OnWaveComplete;
        }

        public void Initialize(ILevelBuilder levelBuilder)
        {
            _levelBuilder = levelBuilder;
        }

        public void StartGame() => TransitionToPhase(GamePhase.PLACING_TOWERS);

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

            DisableTowerHighlights();

            foreach (var tower in _placedTowersThisRound)
            {
                if (tower != selectedTower)
                {
                    ConvertTowerToStone(tower);
                }
            }

            _placedTowersThisRound.Clear();

            // Start the wave
            StartWave();
        }

        public void ConvertTowerToStone(Tower tower)
        {
            if (_levelBuilder == null)
            {
                Debug.LogError("LevelBuilder not set in GameStateService");
                return;
            }

            Vector3 worldPos = tower.transform.position;
            int x = Mathf.RoundToInt(worldPos.x / _levelBuilder.MapData.BlockSize);
            int y = Mathf.RoundToInt(-worldPos.z / _levelBuilder.MapData.BlockSize);

            _levelBuilder.SetTowerType(x, y, TowerType.Stone);

            var enemyTrigger = tower.GetComponent<EnemyTrigger>();
            if (enemyTrigger != null)
            {
                GameObject.Destroy(enemyTrigger);
            }

            tower.enabled = false;

            _towerFactory.ReplaceWithStoneModel(tower);

            Debug.Log($"Tower at ({x}, {y}) converted to stone");
        }

        public bool IsPlacedThisRound(Tower tower)
        {
            return _placedTowersThisRound.Contains(tower);
        }

        public void RemovePlacedThisRound(Tower tower)
        {
            _placedTowersThisRound.Remove(tower);
        }

        public IReadOnlyList<Tower> GetPlacedThisRound()
        {
            return _placedTowersThisRound;
        }

        public void SelectCombinedTower(Tower combinedTower)
        {
            if (_currentPhase != GamePhase.SELECTING_TOWER)
            {
                Debug.LogWarning("SelectCombinedTower called outside SELECTING_TOWER phase");
                return;
            }

            DisableTowerHighlights();
            
            foreach (var tower in _placedTowersThisRound)
            {
                if (tower != null)
                    ConvertTowerToStone(tower);
            }
            _placedTowersThisRound.Clear();

            StartWave();
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
