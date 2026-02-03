namespace infrastructure.services.playerService
{
    public interface IPlayerService
    {
        int PlayerLevel { get; }
        int Lives { get; }

        int GetRandomTowerLevel();
        void AwardExperience(int xp);
        void LoseLife(int amount);
    }
}
