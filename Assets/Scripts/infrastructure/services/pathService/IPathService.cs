using System.Collections.Generic;
using UnityEngine;

namespace infrastructure.services.pathService
{
    public interface IPathService
    {
        List<Vector2Int> CurrentPath { get; }
        List<Vector2Int> FindPath();
    }
}