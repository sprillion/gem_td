using System.Collections.Generic;
using infrastructure.factories.blocks;
using infrastructure.factories.towers;
using infrastructure.services.gameStateService;
using infrastructure.services.inputService;
using infrastructure.services.pathService;
using infrastructure.services.playerSkillService;
using infrastructure.services.resourceProvider;
using infrastructure.services.towerService;
using towers;
using UnityEngine;

namespace level.builder
{
    public class LevelBuilder : ILevelBuilder
    {
        private const string MapPath = "ScriptableObjects/Maps/Map37";

        private readonly IResourceProvider _resourceProvider;
        private readonly IBlockFactory _blockFactory;
        private readonly IInputService _inputService;
        private readonly ITowerService _towerService;
        private readonly ITowerFactory _towerFactory;
        private readonly IPathService _pathService;

        private IGameStateService _gameStateService;
        private IPlayerSkillService _playerSkillService;

        private MapData _mapData;
        private TowerType[,] _towerMap;
        private Dictionary<Vector2Int, Tower> _towerGameObjects = new Dictionary<Vector2Int, Tower>();

        public MapData MapData => _mapData;
        public TowerType[,] TowerMap => _towerMap;

        public LevelBuilder(
            IResourceProvider resourceProvider,
            IBlockFactory blockFactory,
            IInputService inputService,
            ITowerService towerService,
            ITowerFactory towerFactory,
            IPathService pathService
            )
        {
            _resourceProvider = resourceProvider;
            _blockFactory = blockFactory;
            _inputService = inputService;
            _towerService = towerService;
            _towerFactory = towerFactory;
            _pathService = pathService;
            Load();
            CreateTowerMap();
            _inputService.OnSpacePressed += Build;
        }

        public void Initialize(IGameStateService gameStateService, IPlayerSkillService playerSkillService)
        {
            _gameStateService = gameStateService;
            _playerSkillService = playerSkillService;
        }

        public void Build()
        {
            var parent = new GameObject("Map").transform;
            
            for (int i = 0; i < _mapData.Width; i++)
            {
                for (int j = 0; j < _mapData.Height; j++)
                {
                    CreateBlock(i, j, _mapData.BlocksMap[i, j], parent);        
                }
            }
        }

        public Tower CreateTower(int x, int y)
        {
            // Only allow placement during PLACING_TOWERS phase
            if (_gameStateService != null &&
                _gameStateService.CurrentPhase != GamePhase.PLACING_TOWERS)
            {
                Debug.Log("Cannot place towers during " + _gameStateService.CurrentPhase);
                return null;
            }

            if (!CanSetOnBlock(x, y)) return null;
            var towerType = _towerService.GetTowerTypeFromChance();
            _towerMap[x, y] = towerType;
            if (_pathService.FindPath(MapData, TowerMap) == null)
            {
                _towerMap[x, y] = TowerType.None;
                return null;
            }
            var tower = _towerFactory.CreateTower(towerType, _towerService.GetLevelFromChance());
            tower.transform.position = new Vector3(x, 0, -y) * MapData.BlockSize;

            // Track tower GameObject
            _towerGameObjects[new Vector2Int(x, y)] = tower;

            // Register with state service if available
            _gameStateService?.RegisterTowerPlacement(tower);

            // Consume any active chance boosts
            _playerSkillService?.ConsumeChanceBoost();

            return tower;
        }

        public Tower GetTowerAtPosition(int x, int y)
        {
            return _towerGameObjects.TryGetValue(new Vector2Int(x, y), out Tower tower) ? tower : null;
        }

        public void RestoreTower(TowerType towerType, int level, int x, int y)
        {
            // Update tower map
            _towerMap[x, y] = towerType;

            // Create tower GameObject
            Tower tower;
            if (towerType == TowerType.Stone)
            {
                // For stone towers, create a basic tower first (P type, level 0)
                // then replace its model with stone
                tower = _towerFactory.CreateTower(TowerType.P, 0);
                _towerFactory.ReplaceWithStoneModel(tower);

                // Disable combat functionality for stone towers
                var enemyTrigger = tower.GetComponent<EnemyTrigger>();
                if (enemyTrigger != null)
                {
                    UnityEngine.Object.Destroy(enemyTrigger);
                }
                tower.enabled = false;
            }
            else
            {
                // For combat towers, use saved level
                tower = _towerFactory.CreateTower(towerType, level);
            }

            tower.transform.position = new Vector3(x, 0, -y) * MapData.BlockSize;

            // Track tower GameObject
            _towerGameObjects[new Vector2Int(x, y)] = tower;

            Debug.Log($"Restored tower {towerType} (Level {level}) at ({x}, {y})");
        }

        public void SetTowerType(int x, int y, TowerType towerType)
        {
            _towerMap[x, y] = towerType;
        }

        public void SwapTowers(int x1, int y1, int x2, int y2)
        {
            var pos1 = new Vector2Int(x1, y1);
            var pos2 = new Vector2Int(x2, y2);

            _towerGameObjects.TryGetValue(pos1, out Tower tower1);
            _towerGameObjects.TryGetValue(pos2, out Tower tower2);

            // Update dictionary entries
            if (tower1 != null) _towerGameObjects[pos2] = tower1;
            else _towerGameObjects.Remove(pos2);

            if (tower2 != null) _towerGameObjects[pos1] = tower2;
            else _towerGameObjects.Remove(pos1);
        }

        private void Load()
        {
            _mapData = _resourceProvider.Load<MapData>(MapPath);
        }

        private void CreateBlock(int x, int y, BlockType blockType, Transform parent)
        {
            var block = _blockFactory.Create(blockType);
            if (block == null) return;
            block.transform.position = new Vector3(x * _mapData.BlockSize, 0, -y * _mapData.BlockSize);
            block.transform.SetParent(parent);
            block.SetPosition(x, y, _mapData.BlockSize);
        }

        private void CreateTowerMap()
        {
            _towerMap = new TowerType[MapData.Width, MapData.Height];
        }

        private bool CanSetOnBlock(int x, int y)
        {
            switch (MapData.BlocksMap[x, y])
            {
                case BlockType.Dark:
                case BlockType.Light:
                case BlockType.Way:
                    return true;
                case BlockType.None:
                case BlockType.Point:
                case BlockType.NoPut:
                case BlockType.Start:
                case BlockType.End:
                default:
                    return false;
            }
        }
    }
}
