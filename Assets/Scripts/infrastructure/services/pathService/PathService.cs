using System;
using System.Collections.Generic;
using infrastructure.factories.blocks;
using infrastructure.services.towerService;
using level.builder;
using Sirenix.Utilities;
using UnityEngine;

namespace infrastructure.services.pathService
{
    public class PathService : IPathService
    {
        private readonly ILevelBuilder _levelBuilder;
        private readonly ITowerService _towerService;
        private readonly Dictionary<Vector2Int, float> _gMap = new Dictionary<Vector2Int, float>();
        private readonly Dictionary<Vector2Int, float> _hMap = new Dictionary<Vector2Int, float>();
        private readonly Dictionary<Vector2Int, Vector2Int> _parentMap = new Dictionary<Vector2Int, Vector2Int>();
        private readonly HashSet<Vector2Int> _closedList = new HashSet<Vector2Int>();
        private readonly HashSet<Vector2Int> _openList = new HashSet<Vector2Int>();

        private readonly Func<Vector2Int, Vector2Int, float> _heuristic = (nodeA, nodeB) => 
            Math.Abs(nodeA.x - nodeB.x) + Math.Abs(nodeA.y - nodeB.y);

        private readonly int _mapWidth;
        private readonly int _mapHeight;

        private Vector2Int _startNode;
        private Vector2Int _endNode;

        public List<Vector2Int> CurrentPath { get; private set; } = new List<Vector2Int>();

        public PathService(ILevelBuilder levelBuilder, ITowerService towerService)
        {
            _levelBuilder = levelBuilder;
            _towerService = towerService;
            _mapWidth = _towerService.TowerMap.GetLength(0);
            _mapHeight = _towerService.TowerMap.GetLength(1);
            SetStartAndEndNodes();
        }

        public List<Vector2Int> FindPath()
        {
            var points = _levelBuilder.MapData.Points;
            var newPath = new List<Vector2Int>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                var path = FindPath(points[i], points[i + 1]);
                if (path == null) return null;
                newPath.AddRange(path);
            }
            CurrentPath = newPath;
            return CurrentPath;
        }

        private List<Vector2Int> FindPath(Vector2Int startNode, Vector2Int endNode)
        {
            Clear();
            
            _openList.Add(startNode);

            while (_openList.Count > 0)
            {
                var currentNode = GetBestNode();
                if (currentNode.x == endNode.x && currentNode.y == endNode.y)
                {
                    return GeneratePath(currentNode);
                }
                
                _openList.Remove(currentNode);
                _closedList.Add(currentNode);

                var neighbors = GetNeighbors(currentNode);

                _closedList.ForEach(node => neighbors.Remove(node));
                
                foreach (var neighbor in neighbors)
                {
                    var distance = GetDistance(currentNode, neighbor);
                    var tentativeGScore = GetValueFromDictionary(_gMap, currentNode) + distance;

                    if (!_openList.Contains(neighbor) || tentativeGScore < GetValueFromDictionary(_gMap, currentNode))
                    {
                        _parentMap[neighbor] = currentNode;
                        _gMap[neighbor] = tentativeGScore;
                        _hMap[neighbor] = _heuristic(neighbor, endNode);
                        _openList.Add(neighbor);
                    }
                }
            }
            return null;
        }

        private void Clear()
        {
            _openList.Clear();
            _closedList.Clear();
            _hMap.Clear();
            _gMap.Clear();
            _parentMap.Clear();
        }

        private List<Vector2Int> GetNeighbors(Vector2Int node)
        {
            var neighbors = new List<Vector2Int>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    var neighborX = node.x + x;
                    var neighborY = node.y + y;

                    if (neighborX < 0 || neighborX >= _mapWidth || neighborY < 0 || neighborY >= _mapHeight)
                        continue;
                    
                    if ((int)_towerService.TowerMap[neighborX, neighborY] > 0)
                        continue;
                    
                                        
                    if (Math.Abs(x) == Math.Abs(y))
                    {
                        if ((int)_towerService.TowerMap[neighborX, node.y] > 0 && 
                            (int)_towerService.TowerMap[node.x, neighborY] > 0)
                        {
                            continue;
                        }
                    }
                    
                    neighbors.Add(new Vector2Int(neighborX, neighborY));
                }
            }

            return neighbors;
        }

        private Vector2Int GetBestNode()
        {
            var minF = float.MaxValue;
            var newNode = new Vector2Int();
            _openList.ForEach(node =>
            {
                var f = GetValueFromDictionary(_gMap, node) + GetValueFromDictionary(_hMap, node);
                if (f < minF)
                {
                    minF = f;
                    newNode = node;
                }
            });
            return newNode;
        }
        
        private float GetDistance(Vector2Int nodeA, Vector2Int nodeB)
        {
            if (nodeA.x == nodeB.x || nodeA.y == nodeB.y)
            {
                return 10;
            }
            return 16f;
        }

        private List<Vector2Int> GeneratePath(Vector2Int endNode)
        {
            var path = new List<Vector2Int>();
            var currentNode = endNode;
        
            while (_parentMap.ContainsKey(currentNode))
            {
                path.Add(currentNode);
                currentNode = _parentMap[currentNode];
            }
            path.Add(currentNode);
            path.Reverse();
            return path;
        }

        
        private void SetStartAndEndNodes()
        {
            _startNode = NodeOfBlock(BlockType.Start);
            _endNode = NodeOfBlock(BlockType.End);
        }

        private Vector2Int NodeOfBlock(BlockType blockType)
        {
            for (int i = 0; i < _mapWidth; i++)
            {
                for (int j = 0; j < _mapHeight; j++)
                {
                    if (_levelBuilder.MapData.BlocksMap[i, j] == blockType)
                        return new Vector2Int(i, j);
                }
            }
            return default;
        }

        private T GetValueFromDictionary<T>(Dictionary<Vector2Int, T> map, Vector2Int key)
        {
            if (map.TryGetValue(key, out var value))
            {
                return value;
            }

            return default;
        }
    }
}