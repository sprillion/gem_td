using System;
using infrastructure.services.updateService;
using Zenject;

namespace infrastructure.services.timerService
{
    public class TimerService : ITimerService
    {
        private readonly IUpdateService _updateService;

        private float _totalGameTime = 0f;
        private bool _isRunning = false;

        public float TotalGameTime => _totalGameTime;
        public bool IsRunning => _isRunning;

        public string FormattedTotalTime
        {
            get
            {
                int minutes = (int)(_totalGameTime / 60f);
                int seconds = (int)(_totalGameTime % 60f);
                return $"{minutes:D2}:{seconds:D2}";
            }
        }

        public event Action<float> OnTimeUpdated;

        [Inject]
        public TimerService(IUpdateService updateService)
        {
            _updateService = updateService;
            _updateService.OnUpdate += OnUpdate;
        }

        public void StartTimer()
        {
            _isRunning = true;
        }

        public void PauseTimer()
        {
            _isRunning = false;
        }

        public void ResetTimer()
        {
            _totalGameTime = 0f;
            OnTimeUpdated?.Invoke(_totalGameTime);
        }

        private void OnUpdate()
        {
            if (!_isRunning) return;

            _totalGameTime += UnityEngine.Time.deltaTime;
            OnTimeUpdated?.Invoke(_totalGameTime);
        }
    }
}
