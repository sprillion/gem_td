using towers;

namespace infrastructure.factories.towers
{
    public interface ITowerFactory
    {
        Tower CreateTower(TowerType towerType, int towerLevel);
        void ReplaceWithStoneModel(Tower tower);
    }
}