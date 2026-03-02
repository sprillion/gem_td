using infrastructure.services.pathService;
using infrastructure.services.playerService;
using infrastructure.services.playerSkillService;
using towers;
using Random = UnityEngine.Random;
using Zenject;

namespace infrastructure.services.towerService
{
    public class TowerService : ITowerService
    {
        private readonly IPathService _pathService;
        private readonly IPlayerService _playerService;
        private readonly IPlayerSkillService _playerSkillService;

        [Inject]
        public TowerService(IPathService pathService, IPlayerService playerService,
            IPlayerSkillService playerSkillService)
        {
            _pathService = pathService;
            _playerService = playerService;
            _playerSkillService = playerSkillService;
        }

        public int GetLevelFromChance()
        {
            var boost = _playerSkillService.GetActiveLevelChanceBoost();
            if (boost.HasValue)
            {
                float bonusPercent = _playerSkillService.GetLevelChanceBonusPercent();
                if (Random.value * 100f < bonusPercent)
                    return boost.Value;
            }

            return _playerService.GetRandomTowerLevel();
        }

        public TowerType GetTowerTypeFromChance()
        {
            var boost = _playerSkillService.GetActiveTypeChanceBoost();
            if (boost.HasValue)
            {
                float bonusPercent = _playerSkillService.GetTypeChanceBonusPercent();
                if (Random.value * 100f < bonusPercent)
                    return boost.Value;
            }

            return (TowerType)(Random.Range(0, 8) + 2);
        }
    }
}