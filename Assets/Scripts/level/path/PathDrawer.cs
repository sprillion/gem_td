using System.Collections.Generic;
using infrastructure.services.inputService;
using infrastructure.services.pathService;
using infrastructure.services.resourceProvider;
using UnityEngine;

namespace level.path
{
    public class PathDrawer : IPathDrawer
    {
        private const string LinePath = "Prefabs/Path/Line";
        
        private readonly IPathService _pathService;
        private readonly IResourceProvider _resourceProvider;
        private readonly IInputService _inputService;
        private LineRenderer _line;

        public PathDrawer(IPathService pathService, IResourceProvider resourceProvider, IInputService inputService)
        {
            _pathService = pathService;
            _resourceProvider = resourceProvider;
            _inputService = inputService;
            _inputService.OnPPressed += () => DrawPath(_pathService.FindPath());
            Load();
        }

        private void DrawPath(List<AStarNode> nodes)
        {
            _line.positionCount = nodes.Count;
            for (int i = 0; i < nodes.Count; i++)
            {
                var point = new Vector3(nodes[i].X, 0, -nodes[i].Y) * 2;
                _line.SetPosition(i, point);
            }
        }
        
        private void Load()
        {
            _line = _resourceProvider.LoadInstance<LineRenderer>(LinePath, Vector3.up * 0.1f, Quaternion.identity);
        }
    }
}
