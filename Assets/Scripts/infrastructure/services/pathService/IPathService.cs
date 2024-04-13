using System;
using System.Collections.Generic;
using level.builder;
using towers;
using UnityEngine;

namespace infrastructure.services.pathService
{
    public interface IPathService
    {
        List<Vector2Int> CurrentPath { get; }
        event Action<List<Vector2Int>> OnPathChange; 
        List<Vector2Int> FindPath(MapData mapData, TowerType[,] towerMap);
    }
}