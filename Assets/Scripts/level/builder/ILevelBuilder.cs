using infrastructure.services.gameStateService;
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
        void Initialize(IGameStateService gameStateService);
    }
}
