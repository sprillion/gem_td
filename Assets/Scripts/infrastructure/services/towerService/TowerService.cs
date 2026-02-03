using infrastructure.services.pathService;
using infrastructure.services.playerService;
using towers;
using Random = UnityEngine.Random;
using Zenject;

namespace infrastructure.services.towerService
{
    public class TowerService : ITowerService
    {
        private readonly IPathService _pathService;
        private readonly IPlayerService _playerService;

        [Inject]
        public TowerService(IPathService pathService, IPlayerService playerService)
        {
            _pathService = pathService;
            _playerService = playerService;
        }

        public int GetLevelFromChance()
        {
            return _playerService.GetRandomTowerLevel();
        }

        public TowerType GetTowerTypeFromChance()
        {
            return (TowerType)(Random.Range(0, 8) + 2);
        }
    }
}