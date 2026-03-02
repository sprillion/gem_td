using System;
using infrastructure.services.resourceProvider;
using UnityEngine;
using Zenject;

namespace infrastructure.services.playerService
{
    public class PlayerService : IPlayerService
    {
        private readonly PlayerProgressionData _progressionData;

        private int _playerLevel = 1;
        private int _currentXP = 0;
        private int _lives = 100;
        private int _gold = 0;

        public int PlayerLevel => _playerLevel;
        public int Lives => _lives;
        public int CurrentXP => _currentXP;
        public int XPForNextLevel => GetXPForLevel(_playerLevel);
        public int Gold => _gold;

        public event Action<int> OnLevelChanged;
        public event Action<int> OnXPChanged;
        public event Action<int> OnGoldChanged;
        public event Action<int> OnLivesChanged;

        [Inject]
        public PlayerService(IResourceProvider resourceProvider)
        {
            _progressionData = resourceProvider.Load<PlayerProgressionData>("ScriptableObjects/PlayerProgression");
            if (_progressionData == null)
                Debug.LogWarning("PlayerProgressionData not found at Resources/ScriptableObjects/PlayerProgression. Using defaults.");
        }

        private int GetXPForLevel(int level)
        {
            if (_progressionData == null)
                return level * 100;
            int idx = level - 1;
            return idx < _progressionData.XPPerLevel.Length ? _progressionData.XPPerLevel[idx] : int.MaxValue;
        }

        public float[] GetTowerLevelBaseWeights()
        {
            if (_progressionData == null || _progressionData.TowerLevelWeights == null ||
                _progressionData.TowerLevelWeights.Length == 0)
                return new float[] { 0.5f, 0.25f, 0.15f, 0.10f, 0f };

            int idx = Mathf.Clamp(_playerLevel - 1, 0, _progressionData.TowerLevelWeights.Length - 1);
            return _progressionData.TowerLevelWeights[idx].Weights;
        }

        public int GetRandomTowerLevel()
        {
            var weights = GetTowerLevelBaseWeights();

            float total = 0f;
            foreach (var w in weights) total += w;
            if (total <= 0f) return 0;

            float roll = UnityEngine.Random.value * total;
            float cumulative = 0f;
            for (int i = 0; i < weights.Length; i++)
            {
                cumulative += weights[i];
                if (roll < cumulative) return i;
            }
            return weights.Length - 1;
        }

        public void AwardExperience(int xp)
        {
            _currentXP += xp;
            Debug.Log($"Awarded {xp} XP. Total: {_currentXP}");
            OnXPChanged?.Invoke(_currentXP);

            int xpForLevel = GetXPForLevel(_playerLevel);
            if (_currentXP >= xpForLevel && xpForLevel != int.MaxValue)
            {
                _playerLevel++;
                _currentXP -= xpForLevel;
                Debug.Log($"Level up! Now level {_playerLevel}");
                OnLevelChanged?.Invoke(_playerLevel);
                OnXPChanged?.Invoke(_currentXP);
            }
        }

        public void LoseLife(int amount)
        {
            _lives -= amount;
            Debug.Log($"Lost {amount} life. Lives remaining: {_lives}");
            OnLivesChanged?.Invoke(_lives);

            if (_lives <= 0)
                Debug.Log("Game Over!");
        }

        public void AddLives(int amount)
        {
            _lives += amount;
            Debug.Log($"Restored {amount} lives. Lives: {_lives}");
            OnLivesChanged?.Invoke(_lives);
        }

        public void AddGold(int amount)
        {
            _gold += amount;
            Debug.Log($"Added {amount} gold. Total: {_gold}");
            OnGoldChanged?.Invoke(_gold);
        }

        public void LoadPlayerData(int level, int xp, int lives, int gold)
        {
            _playerLevel = level;
            _currentXP = xp;
            _lives = lives;
            _gold = gold;

            Debug.Log($"Player data loaded: Level {_playerLevel}, XP {_currentXP}, Lives {_lives}, Gold {_gold}");

            OnLevelChanged?.Invoke(_playerLevel);
            OnXPChanged?.Invoke(_currentXP);
            OnLivesChanged?.Invoke(_lives);
            OnGoldChanged?.Invoke(_gold);
        }
    }
}
