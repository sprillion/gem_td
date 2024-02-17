using towers;

namespace infrastructure.services.towerService
{
    public interface ITowerService
    {
        TowerType[,] TowerMap { get; }
        Tower SetTower(int x, int y);
    }
}