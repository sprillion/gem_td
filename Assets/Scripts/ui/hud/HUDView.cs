using infrastructure.services.gameStateService;
using infrastructure.services.playerService;
using infrastructure.services.timerService;
using infrastructure.services.updateService;
using infrastructure.services.waveService;
using TMPro;
using UnityEngine;
using Zenject;

namespace ui.hud
{
    public class HUDView : MonoBehaviour
    {
        [Header("Player Info")]
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _xpText;
        [SerializeField] private TextMeshProUGUI _goldText;
        [SerializeField] private TextMeshProUGUI _livesText;
        [SerializeField] private TextMeshProUGUI _totalTimeText;

        [Header("Wave Info")]
        [SerializeField] private TextMeshProUGUI _waveNumberText;
        [SerializeField] private TextMeshProUGUI _waveTimerText;
        [SerializeField] private TextMeshProUGUI _enemyCountText;
        [SerializeField] private TextMeshProUGUI _phaseText;

        private IPlayerService _playerService;
        private IWaveService _waveService;
        private ITimerService _timerService;
        private IGameStateService _gameStateService;
        private IUpdateService _updateService;

        [Inject]
        private void Construct(
            IPlayerService playerService,
            IWaveService waveService,
            ITimerService timerService,
            IGameStateService gameStateService,
            IUpdateService updateService)
        {
            _playerService = playerService;
            _waveService = waveService;
            _timerService = timerService;
            _gameStateService = gameStateService;
            _updateService = updateService;
        }

        private void Start()
        {
            SubscribeToEvents();
            UpdateAllUI();
            _timerService.StartTimer();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            _playerService.OnLevelChanged += OnLevelChanged;
            _playerService.OnXPChanged += OnXPChanged;
            _playerService.OnGoldChanged += OnGoldChanged;
            _playerService.OnLivesChanged += OnLivesChanged;

            _waveService.OnWaveStarted += OnWaveStarted;
            _waveService.OnWaveComplete += OnWaveComplete;
            _waveService.OnEnemyCountChanged += OnEnemyCountChanged;

            _timerService.OnTimeUpdated += OnTimeUpdated;

            _gameStateService.OnPhaseChanged += OnPhaseChanged;

            _updateService.OnUpdate += OnUpdate;
        }

        private void UnsubscribeFromEvents()
        {
            if (_playerService != null)
            {
                _playerService.OnLevelChanged -= OnLevelChanged;
                _playerService.OnXPChanged -= OnXPChanged;
                _playerService.OnGoldChanged -= OnGoldChanged;
                _playerService.OnLivesChanged -= OnLivesChanged;
            }

            if (_waveService != null)
            {
                _waveService.OnWaveStarted -= OnWaveStarted;
                _waveService.OnWaveComplete -= OnWaveComplete;
                _waveService.OnEnemyCountChanged -= OnEnemyCountChanged;
            }

            if (_timerService != null)
            {
                _timerService.OnTimeUpdated -= OnTimeUpdated;
            }

            if (_gameStateService != null)
            {
                _gameStateService.OnPhaseChanged -= OnPhaseChanged;
            }

            if (_updateService != null)
            {
                _updateService.OnUpdate -= OnUpdate;
            }
        }

        private void UpdateAllUI()
        {
            UpdateLevelText();
            UpdateXPText();
            UpdateGoldText();
            UpdateLivesText();
            UpdateTotalTimeText();
            UpdateWaveNumberText();
            UpdateWaveTimerText();
            UpdateEnemyCountText();
            UpdatePhaseText();
        }

        private void OnUpdate()
        {
            if (_waveService.IsWaveInProgress)
            {
                UpdateWaveTimerText();
            }
        }

        private void OnLevelChanged(int level) => UpdateLevelText();
        private void OnXPChanged(int xp) => UpdateXPText();
        private void OnGoldChanged(int gold) => UpdateGoldText();
        private void OnLivesChanged(int lives) => UpdateLivesText();
        private void OnTimeUpdated(float time) => UpdateTotalTimeText();

        private void OnWaveStarted()
        {
            UpdateWaveNumberText();
            UpdateWaveTimerText();
        }

        private void OnWaveComplete()
        {
            UpdateWaveTimerText();
        }

        private void OnEnemyCountChanged(int count) => UpdateEnemyCountText();
        private void OnPhaseChanged(GamePhase phase) => UpdatePhaseText();

        private void UpdateLevelText()
        {
            if (_levelText != null)
                _levelText.text = $"{_playerService.PlayerLevel}";
        }

        private void UpdateXPText()
        {
            if (_xpText != null)
                _xpText.text = $"{_playerService.CurrentXP}/{_playerService.XPForNextLevel}";
        }

        private void UpdateGoldText()
        {
            if (_goldText != null)
                _goldText.text = $"{_playerService.Gold}";
        }

        private void UpdateLivesText()
        {
            if (_livesText != null)
                _livesText.text = $"{_playerService.Lives}";
        }

        private void UpdateTotalTimeText()
        {
            if (_totalTimeText != null)
                _totalTimeText.text = $"Время: {_timerService.FormattedTotalTime}";
        }

        private void UpdateWaveNumberText()
        {
            if (_waveNumberText != null)
                _waveNumberText.text = $"{_waveService.CurrentWaveNumber}";
        }

        private void UpdateWaveTimerText()
        {
            if (_waveTimerText != null)
            {
                float elapsed = _waveService.WaveElapsedTime;
                int minutes = (int)(elapsed / 60f);
                int seconds = (int)(elapsed % 60f);
                _waveTimerText.text = $"Время волны: {minutes:D2}:{seconds:D2}";
            }
        }

        private void UpdateEnemyCountText()
        {
            if (_enemyCountText != null)
                _enemyCountText.text = $"{_waveService.LivingEnemyCount}";
        }

        private void UpdatePhaseText()
        {
            if (_phaseText != null)
            {
                string phaseName = _gameStateService.CurrentPhase switch
                {
                    GamePhase.SKILL_SELECTION => "Выбор способностей",
                    GamePhase.PLACING_TOWERS => "Установка башни",
                    GamePhase.SELECTING_TOWER => "Выбор башни",
                    GamePhase.COMBAT => "Сражение",
                    GamePhase.GAME_OVER => "Конец игры",
                    _ => "Неизвестно"
                };
                _phaseText.text = $"Фаза: {phaseName}";
            }
        }
    }
}
