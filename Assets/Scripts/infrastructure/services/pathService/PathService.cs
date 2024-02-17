using System;
using System.Collections.Generic;
using infrastructure.factories.blocks;
using infrastructure.services.towerService;
using level.builder;

namespace infrastructure.services.pathService
{
    public class PathService : IPathService
    {
        private readonly ILevelBuilder _levelBuilder;
        private readonly ITowerService _towerService;
        private readonly List<AStarNode> _openList = new List<AStarNode>();
        private readonly List<AStarNode> _closedList = new List<AStarNode>();
        
        // private readonly Dictionary<AStarNode, bool> _openList = new Dictionary<AStarNode, bool>();
        // private readonly Dictionary<AStarNode, bool> _closedList = new Dictionary<AStarNode, bool>();

        private AStarNode _startNode;
        private AStarNode _endNode;

        private readonly int _mapWidth;
        private readonly int _mapHeight;

        public PathService(ILevelBuilder levelBuilder, ITowerService towerService)
        {
            _levelBuilder = levelBuilder;
            _towerService = towerService;
            _mapWidth = _towerService.TowerMap.GetLength(0);
            _mapHeight = _towerService.TowerMap.GetLength(1);
            SetStartAndEndNodes();
        }

        public List<AStarNode> FindPath()
        {
            _openList.Clear();
            _closedList.Clear();

            _openList.Add(_startNode);

            while (_openList.Count > 0)
            {
                var currentNode = GetLowestCostNode(_openList);
                _openList.Remove(currentNode);
                _closedList.Add(currentNode);

                if (currentNode.X == _endNode.X && currentNode.Y == _endNode.Y)
                {
                    return GeneratePath(currentNode);
                }

                var neighbors = GetNeighbors(currentNode);

                foreach (var neighbor in neighbors)
                {
                    if (_closedList.Contains(neighbor)) // Проверка, является ли узел непроходимым
                        continue;
                    
                    var tentativeGScore = currentNode.G + GetDistance(currentNode, neighbor);

                    if (!_openList.Contains(neighbor) || tentativeGScore < neighbor.G)
                    {
                        neighbor.Parent = currentNode;
                        neighbor.G = tentativeGScore;
                        neighbor.H = Heuristic(neighbor, _endNode);

                        if (!_openList.Contains(neighbor))
                            _openList.Add(neighbor);
                    }
                }
            }

            return null; // Путь не найден
        }

        private AStarNode GetLowestCostNode(List<AStarNode> nodeList)
        {
            var lowestCost = double.MaxValue;
            AStarNode lowestCostNode = null;

            foreach (var node in nodeList)
            {
                if (node.F < lowestCost)
                {
                    lowestCost = node.F;
                    lowestCostNode = node;
                }
            }

            return lowestCostNode;
        }

        private List<AStarNode> GetNeighbors(AStarNode node)
        {
            var neighbors = new List<AStarNode>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    var neighborX = node.X + x;
                    var neighborY = node.Y + y;

                    if (neighborX < 0 || neighborX >= _mapWidth || neighborY < 0 || neighborY >= _mapHeight)
                        continue;
                    
                    if ((int)_towerService.TowerMap[neighborX, neighborY] > 0)
                        continue;
                    
                                        
                    if (Math.Abs(x) == Math.Abs(y))
                    {
                        if ((int)_towerService.TowerMap[neighborX, node.Y] > 0 && 
                            (int)_towerService.TowerMap[node.X, neighborY] > 0)
                        {
                            continue;
                        }
                    }
                    
                    neighbors.Add(new AStarNode(neighborX, neighborY));
                }
            }

            return neighbors;
        }

        private double GetDistance(AStarNode nodeA, AStarNode nodeB)
        {
            return Math.Sqrt(Math.Pow(nodeB.X - nodeA.X, 2) + Math.Pow(nodeB.Y - nodeA.Y, 2));
        }

        private Func<AStarNode, AStarNode, double> Heuristic = (nodeA, nodeB) => 
            Math.Abs(nodeA.X - nodeB.X) + Math.Abs(nodeA.Y - nodeB.Y);

        private List<AStarNode> GeneratePath(AStarNode endNode)
        {
            var path = new List<AStarNode>();
            var currentNode = endNode;

            while (currentNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }

        private void SetStartAndEndNodes()
        {
            _startNode = NodeOfBlock(BlockType.Start);
            _endNode = NodeOfBlock(BlockType.End);
        }

        private AStarNode NodeOfBlock(BlockType blockType)
        {
            for (int i = 0; i < _mapWidth; i++)
            {
                for (int j = 0; j < _mapHeight; j++)
                {
                    if (_levelBuilder.MapData.BlocksMap[i, j] == blockType)
                        return new AStarNode(i, j);
                }
            }
            return null;
        }

    }
    
    public class AStarNode
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double G { get; set; } // Стоимость пути от начального узла до этого узла
        public double H { get; set; } // Эвристическая оценка расстояния от этого узла до целевого узла
        public double F => G + H; // Общая оценка

        public AStarNode Parent { get; set; } // Родительский узел

        public AStarNode(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         int[,] map = {
    //             {0, 0, 0, 0, 0},
    //             {0, 1, 1, 1, 0},
    //             {0, 0, 0, 0, 0},
    //             {0, 1, 1, 1, 0},
    //             {0, 0, 0, 0, 0}
    //         };
    //
    //         int startX = 0;
    //         int startY = 0;
    //         int endX = 4;
    //         int endY = 4;
    //
    //         Func<AStarNode, AStarNode, double> heuristic = (nodeA, nodeB) =>
    //         {
    //             return Math.Abs(nodeA.X - nodeB.X) + Math.Abs(nodeA.Y - nodeB.Y); // Манхэттенское расстояние
    //         };
    //
    //         var astar = new AStar(map, startX, startY, endX, endY, heuristic);
    //         var path = astar.FindPath();
    //
    //         if (path != null)
    //         {
    //             foreach (var node in path)
    //             {
    //                 Console.WriteLine($"({node.X}, {node.Y})");
    //             }
    //         }
    //         else
    //         {
    //             Console.WriteLine("Путь не найден.");
    //         }
    //     }
    // }
}