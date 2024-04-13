using infrastructure.services.pathService;
using towers;
using Random = UnityEngine.Random;

namespace infrastructure.services.towerService
{
    public class TowerService : ITowerService
    {
        private readonly IPathService _pathService;

        public TowerService(IPathService pathService)
        {
            _pathService = pathService;
        }

        public int GetLevelFromChance()
        {
            return 0;
        }

        public TowerType GetTowerTypeFromChance()
        {
            return (TowerType)(Random.Range(0, 8) + 2);
        }
    }
}