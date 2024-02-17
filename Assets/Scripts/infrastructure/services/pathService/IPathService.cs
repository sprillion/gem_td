using System.Collections.Generic;

namespace infrastructure.services.pathService
{
    public interface IPathService
    {
        List<AStarNode> FindPath();
    }
}