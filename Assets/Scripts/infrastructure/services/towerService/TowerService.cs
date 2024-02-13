using System;
using infrastructure.factories.blocks;
using infrastructure.factories.towers;
using level.builder;
using towers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace infrastructure.services.towerService
{
    public class TowerService : ITowerService
    {
        private readonly ITowerFactory _towerFactory;
        private readonly ILevelBuilder _levelBuilder;

        private TowerType[,] _towerMap;
        
        public TowerService(ITowerFactory towerFactory, ILevelBuilder levelBuilder)
        {
            _towerFactory = towerFactory;
            _levelBuilder = levelBuilder;
            CreateTowerMap();
        }

        public Tower SetTower(int x, int y)
        {
            if (!CanSetOnBlock(x, y)) return null;
            var towerType = GetTowerTypeFromChance();
            var tower = _towerFactory.CreateTower(towerType, GetLevelFromChance());
            _towerMap[x, y] = towerType;
            tower.transform.position = new Vector3(x, 0, -y) * _levelBuilder.MapData.BlockSize; 
            return tower;
        }

        private void CreateTowerMap()
        {
            _towerMap = new TowerType[_levelBuilder.MapData.Width, _levelBuilder.MapData.Height];
            Debug.Log(_towerMap[0, 0]);
        }

        private int GetLevelFromChance()
        {
            return 0;
        }

        private TowerType GetTowerTypeFromChance()
        {
            return (TowerType)(Random.Range(0, 8) + 2);
        }

        private bool CanSetOnBlock(int x, int y)
        {
            switch (_levelBuilder.MapData.BlocksMap[x, y])
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