using infrastructure.factories.blocks;
using infrastructure.factories.towers;
using infrastructure.services.inputService;
using infrastructure.services.pathService;
using infrastructure.services.resourceProvider;
using infrastructure.services.towerService;
using towers;
using UnityEngine;

namespace level.builder
{
    public class LevelBuilder : ILevelBuilder
    {
        private const string MapPath = "ScriptableObjects/Maps/Map";
        
        private readonly IResourceProvider _resourceProvider;
        private readonly IBlockFactory _blockFactory;
        private readonly IInputService _inputService;
        private readonly ITowerService _towerService;
        private readonly ITowerFactory _towerFactory;
        private readonly IPathService _pathService;

        private MapData _mapData;
        private TowerType[,] _towerMap;
        
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
            return tower;
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
