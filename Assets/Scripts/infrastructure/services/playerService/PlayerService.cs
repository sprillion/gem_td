using System;
using UnityEngine;

namespace infrastructure.services.playerService
{
    public class PlayerService : IPlayerService
    {
        private int _playerLevel = 1;
        private int _currentXP = 0;
        private int _lives = 20;
        private int _gold = 0;

        public int PlayerLevel => _playerLevel;
        public int Lives => _lives;
        public int CurrentXP => _currentXP;
        public int XPForNextLevel => _playerLevel * 100;
        public int Gold => _gold;

        public event Action<int> OnLevelChanged;
        public event Action<int> OnXPChanged;
        public event Action<int> OnGoldChanged;
        public event Action<int> OnLivesChanged;

        public int GetRandomTowerLevel()
        {
            // For now, return a simple weighted random level based on player level
            // Level 0 is most common, higher levels less common
            // Formula: weighted by player level

            float roll = UnityEngine.Random.value;

            // 50% chance for level 0
            if (roll < 0.5f) return 0;

            // 25% chance for level 1
            if (roll < 0.75f) return Mathf.Min(1, _playerLevel);

            // 15% chance for level 2
            if (roll < 0.9f) return Mathf.Min(2, _playerLevel);

            // 10% chance for higher level
            return Mathf.Min(3, _playerLevel);
        }

        public void AwardExperience(int xp)
        {
            _currentXP += xp;
            Debug.Log($"Awarded {xp} XP. Total: {_currentXP}");
            OnXPChanged?.Invoke(_currentXP);

            // Simple level up: every 100 XP
            int xpForLevel = _playerLevel * 100;
            if (_currentXP >= xpForLevel)
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
            {
                Debug.Log("Game Over!");
                // TODO: Trigger game over state
            }
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

            // Fire events for UI updates
            OnLevelChanged?.Invoke(_playerLevel);
            OnXPChanged?.Invoke(_currentXP);
            OnLivesChanged?.Invoke(_lives);
            OnGoldChanged?.Invoke(_gold);
        }
    }
}
