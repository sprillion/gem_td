using System;
using level.builder;
using towers;

namespace infrastructure.services.gameStateService
{
    public interface IGameStateService
    {
        GamePhase CurrentPhase { get; }
        int TowersPlacedThisRound { get; }
        event Action<GamePhase> OnPhaseChanged;

        void Initialize(ILevelBuilder levelBuilder);
        void StartGame();
        void RegisterTowerPlacement(Tower tower);
        void SelectTower(Tower selectedTower);
        bool IsPlacedThisRound(Tower tower);
        void StartWave();
        void EndWave();
    }
}
