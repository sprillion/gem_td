using System;

namespace infrastructure.services.playerService
{
    public interface IPlayerService
    {
        int PlayerLevel { get; }
        int Lives { get; }
        int CurrentXP { get; }
        int XPForNextLevel { get; }
        int Gold { get; }

        event Action<int> OnLevelChanged;
        event Action<int> OnXPChanged;
        event Action<int> OnGoldChanged;
        event Action<int> OnLivesChanged;

        int GetRandomTowerLevel();
        void AwardExperience(int xp);
        void LoseLife(int amount);
        void AddGold(int amount);
        void LoadPlayerData(int level, int xp, int lives, int gold);
    }
}
