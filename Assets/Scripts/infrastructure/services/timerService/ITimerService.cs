using System;

namespace infrastructure.services.timerService
{
    public interface ITimerService
    {
        float TotalGameTime { get; }
        string FormattedTotalTime { get; }
        bool IsRunning { get; }

        event Action<float> OnTimeUpdated;

        void StartTimer();
        void PauseTimer();
        void ResetTimer();
    }
}
