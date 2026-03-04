using System.Collections.Generic;
using skills;
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
        public SkillSaveData[] equippedSkills;
        public string saveDate;
    }

    [System.Serializable]
    public class SkillSaveData
    {
        public PlayerSkillType skillType;
        public int upgradeLevel;
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
