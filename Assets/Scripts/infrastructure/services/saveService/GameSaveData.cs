using System.Collections.Generic;
using towers;

namespace infrastructure.services.saveService
{
    [System.Serializable]
    public class GameSaveData
    {
        public int version = 1;
        public int lastCompletedWave;
        public PlayerData playerData;
        public List<TowerSaveData> placedTowers;
        public string saveDate;  // ISO 8601 timestamp
    }

    [System.Serializable]
    public class PlayerData
    {
        public int playerLevel;
        public int currentXP;
        public int lives;
        public int gold;
    }

    [System.Serializable]
    public class TowerSaveData
    {
        public TowerType towerType;
        public int level;
        public int gridX;
        public int gridY;
    }
}
