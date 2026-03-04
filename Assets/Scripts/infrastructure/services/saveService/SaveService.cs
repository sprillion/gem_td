using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using infrastructure.services.playerService;
using infrastructure.services.playerSkillService;
using infrastructure.services.waveService;
using level.builder;
using towers;

namespace infrastructure.services.saveService
{
    public class SaveService : ISaveService
    {
        private const string SAVE_KEY = "GemTD_GameSave";

        private readonly IPlayerService _playerService;
        private readonly IWaveService _waveService;
        private readonly ILevelBuilder _levelBuilder;
        private readonly IPlayerSkillService _playerSkillService;

        public bool HasSaveData => PlayerPrefs.HasKey(SAVE_KEY);

        public event Action<GameSaveData> OnGameSaved;
        public event Action<GameSaveData> OnGameLoaded;

        [Inject]
        public SaveService(IPlayerService playerService, IWaveService waveService, ILevelBuilder levelBuilder, IPlayerSkillService playerSkillService)
        {
            _playerService = playerService;
            _waveService = waveService;
            _levelBuilder = levelBuilder;
            _playerSkillService = playerSkillService;
        }

        public void SaveGame()
        {
            try
            {
                var saveData = new GameSaveData
                {
                    version = 1,
                    lastCompletedWave = _waveService.CurrentWaveNumber,
                    playerData = new PlayerData
                    {
                        playerLevel = _playerService.PlayerLevel,
                        currentXP = _playerService.CurrentXP,
                        lives = _playerService.Lives,
                        gold = _playerService.Gold
                    },
                    placedTowers = CollectPlacedTowers(),
                    equippedSkills = CollectEquippedSkills(),
                    saveDate = DateTime.UtcNow.ToString("o") // ISO 8601 format
                };

                string json = JsonUtility.ToJson(saveData, true);
                PlayerPrefs.SetString(SAVE_KEY, json);
                PlayerPrefs.Save();

                Debug.Log($"Game saved: Wave {saveData.lastCompletedWave}, {saveData.placedTowers.Count} towers");
                OnGameSaved?.Invoke(saveData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }

        public GameSaveData LoadGame()
        {
            if (!HasSaveData)
            {
                Debug.LogWarning("No save data found");
                return null;
            }

            try
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

                if (saveData == null || saveData.playerData == null)
                {
                    Debug.LogError("Save data is corrupted");
                    return null;
                }

                Debug.Log($"Game loaded: Wave {saveData.lastCompletedWave}, {saveData.placedTowers?.Count ?? 0} towers");
                OnGameLoaded?.Invoke(saveData);
                return saveData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                return null;
            }
        }

        public bool TryLoadGame(out GameSaveData saveData)
        {
            saveData = LoadGame();
            return saveData != null;
        }

        public void DeleteSave()
        {
            if (HasSaveData)
            {
                PlayerPrefs.DeleteKey(SAVE_KEY);
                PlayerPrefs.Save();
                Debug.Log("Save data deleted");
            }
        }

        private SkillSaveData[] CollectEquippedSkills()
        {
            var equipped = _playerSkillService.EquippedSkills;
            var result = new SkillSaveData[equipped.Count];
            for (int i = 0; i < equipped.Count; i++)
            {
                result[i] = equipped[i] != null
                    ? new SkillSaveData { skillType = equipped[i].Data.SkillType, upgradeLevel = equipped[i].UpgradeLevel }
                    : new SkillSaveData { skillType = skills.PlayerSkillType.None, upgradeLevel = 0 };
            }
            return result;
        }

        private List<TowerSaveData> CollectPlacedTowers()
        {
            var towers = new List<TowerSaveData>();
            var towerMap = _levelBuilder.TowerMap;
            var mapData = _levelBuilder.MapData;

            if (towerMap == null || mapData == null)
            {
                return towers;
            }

            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    TowerType towerType = towerMap[x, y];
                    if (towerType != TowerType.None)
                    {
                        Tower towerComponent = _levelBuilder.GetTowerAtPosition(x, y);
                        int level = 0;

                        // Get level from Tower component (0 for stone towers)
                        if (towerComponent != null && towerType != TowerType.Stone)
                        {
                            level = towerComponent.TowerData?.Level ?? 0;
                        }

                        towers.Add(new TowerSaveData
                        {
                            towerType = towerType,
                            level = level,
                            gridX = x,
                            gridY = y
                        });
                    }
                }
            }

            return towers;
        }
    }
}
