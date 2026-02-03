using System;
using enemies;

namespace infrastructure.services.waveService
{
    public interface IWaveService
    {
        int CurrentWaveNumber { get; }
        event Action OnWaveComplete;

        void StartWave();
        void RegisterEnemyDeath(Enemy enemy);
        void RegisterEnemyReachedEnd(Enemy enemy);
    }
}
