using towers;

namespace infrastructure.services.towerService
{
    public interface ITowerService
    {
        TowerType GetTowerTypeFromChance();
        int GetLevelFromChance();
    }
}