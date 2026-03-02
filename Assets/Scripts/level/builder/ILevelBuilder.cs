using infrastructure.services.gameStateService;
using infrastructure.services.playerSkillService;
using towers;

namespace level.builder
{
    public interface ILevelBuilder
    {
        MapData MapData { get; }
        TowerType[,] TowerMap { get; }
        void Build();
        Tower CreateTower(int x, int y);
        void SetTowerType(int x, int y, TowerType towerType);
        void Initialize(IGameStateService gameStateService, IPlayerSkillService playerSkillService);
        Tower GetTowerAtPosition(int x, int y);
        void RestoreTower(TowerType towerType, int level, int x, int y);
        void SwapTowers(int x1, int y1, int x2, int y2);
    }
}
