using System;
using enemies;
using level.builder;

namespace infrastructure.services.waveService
{
    public interface IWaveService
    {
        int CurrentWaveNumber { get; }
        int LivingEnemyCount { get; }
        float WaveElapsedTime { get; }
        bool IsWaveInProgress { get; }

        event Action OnWaveComplete;
        event Action OnWaveStarted;
        event Action<int> OnEnemyCountChanged;

        void Initialize(ILevelBuilder levelBuilder);
        void StartWave();
        void RegisterEnemyDeath(Enemy enemy);
        void RegisterEnemyReachedEnd(Enemy enemy);
        void SetWaveNumber(int waveNumber);
    }
}
