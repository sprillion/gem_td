using System;
using enemies;

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

        void StartWave();
        void RegisterEnemyDeath(Enemy enemy);
        void RegisterEnemyReachedEnd(Enemy enemy);
        void SetWaveNumber(int waveNumber);
    }
}
