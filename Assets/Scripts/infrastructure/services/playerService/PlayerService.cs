using UnityEngine;

namespace infrastructure.services.playerService
{
    public class PlayerService : IPlayerService
    {
        private int _playerLevel = 1;
        private int _currentXP = 0;
        private int _lives = 20;

        public int PlayerLevel => _playerLevel;
        public int Lives => _lives;

        public int GetRandomTowerLevel()
        {
            // For now, return a simple weighted random level based on player level
            // Level 0 is most common, higher levels less common
            // Formula: weighted by player level

            float roll = Random.value;

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

            // Simple level up: every 100 XP
            int xpForLevel = _playerLevel * 100;
            if (_currentXP >= xpForLevel)
            {
                _playerLevel++;
                _currentXP -= xpForLevel;
                Debug.Log($"Level up! Now level {_playerLevel}");
            }
        }

        public void LoseLife(int amount)
        {
            _lives -= amount;
            Debug.Log($"Lost {amount} life. Lives remaining: {_lives}");

            if (_lives <= 0)
            {
                Debug.Log("Game Over!");
                // TODO: Trigger game over state
            }
        }
    }
}
