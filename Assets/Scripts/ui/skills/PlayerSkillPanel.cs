using infrastructure.services.gameStateService;
using infrastructure.services.inputService;
using infrastructure.services.playerService;
using infrastructure.services.playerSkillService;
using infrastructure.services.selectionService;
using skills;
using TMPro;
using UnityEngine;
using Zenject;

namespace ui.skills
{
    public class PlayerSkillPanel : MonoBehaviour
    {
        [SerializeField] private PlayerSkillButton[] _skillButtons = new PlayerSkillButton[4];
        [SerializeField] private TextMeshProUGUI _targetingPrompt;

        private IPlayerSkillService _playerSkillService;
        private ISelectionService _selectionService;
        private IGameStateService _gameStateService;
        private IPlayerService _playerService;
        private IInputService _inputService;

        [Inject]
        private void Construct(IPlayerSkillService playerSkillService, ISelectionService selectionService,
            IGameStateService gameStateService, IPlayerService playerService, IInputService inputService)
        {
            _playerSkillService = playerSkillService;
            _selectionService = selectionService;
            _gameStateService = gameStateService;
            _playerService = playerService;
            _inputService = inputService;
        }

        private void Start()
        {
            SubscribeToEvents();
            RefreshAll();
            HideTargetingPrompt();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            for (int i = 0; i < _skillButtons.Length; i++)
            {
                if (_skillButtons[i] == null) continue;
                int slotIndex = i;
                _skillButtons[i].OnClicked += () => OnSkillButtonClicked(slotIndex);
            }

            _playerSkillService.OnSkillActivated += OnSkillActivated;
            _playerSkillService.OnSkillCooldownUpdated += OnCooldownUpdated;
            _playerSkillService.OnSkillsChanged += RefreshAll;
            _playerSkillService.OnTargetingStarted += OnTargetingStarted;
            _playerSkillService.OnTargetingCancelled += OnTargetingCancelled;

            _selectionService.OnTowerSelected += OnTowerSelected;

            _inputService.OnSkillKeyPressed += TryActivateSkillByIndex;

            _gameStateService.OnPhaseChanged += OnPhaseChanged;
            _playerService.OnGoldChanged += OnGoldChanged;
        }

        private void UnsubscribeFromEvents()
        {
            if (_playerSkillService != null)
            {
                _playerSkillService.OnSkillActivated -= OnSkillActivated;
                _playerSkillService.OnSkillCooldownUpdated -= OnCooldownUpdated;
                _playerSkillService.OnSkillsChanged -= RefreshAll;
                _playerSkillService.OnTargetingStarted -= OnTargetingStarted;
                _playerSkillService.OnTargetingCancelled -= OnTargetingCancelled;
            }

            if (_selectionService != null)
            {
                _selectionService.OnTowerSelected -= OnTowerSelected;
            }

            if (_inputService != null)
            {
                _inputService.OnSkillKeyPressed -= TryActivateSkillByIndex;
            }

            if (_gameStateService != null)
            {
                _gameStateService.OnPhaseChanged -= OnPhaseChanged;
            }

            if (_playerService != null)
            {
                _playerService.OnGoldChanged -= OnGoldChanged;
            }
        }

        private void OnSkillButtonClicked(int slotIndex)
        {
            if (_playerSkillService.IsTargeting)
            {
                _playerSkillService.CancelTargeting();
                return;
            }

            _playerSkillService.ActivateSkill(slotIndex);
        }

        private void TryActivateSkillByIndex(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < _skillButtons.Length)
                OnSkillButtonClicked(slotIndex);
        }

        private void OnTowerSelected(towers.Tower tower)
        {
            if (_playerSkillService.IsTargeting)
            {
                _playerSkillService.OnTargetSelected(tower);
            }
        }

        private void OnSkillActivated(int slotIndex)
        {
            RefreshAll();
        }

        private void OnCooldownUpdated(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _skillButtons.Length) return;
            if (_skillButtons[slotIndex] == null) return;

            var skill = _playerSkillService.EquippedSkills[slotIndex];
            if (skill == null) return;

            _skillButtons[slotIndex].UpdateCooldown(skill.CooldownRemaining, skill.Cooldown);
            _skillButtons[slotIndex].SetInteractable(_playerSkillService.CanActivateSkill(slotIndex));
        }

        private void OnTargetingStarted(int slotIndex, SkillTargetMode mode)
        {
            ShowTargetingPrompt(mode);
            RefreshAll();
        }

        private void OnTargetingCancelled()
        {
            HideTargetingPrompt();
            RefreshAll();
        }

        private void OnPhaseChanged(GamePhase phase)
        {
            RefreshAll();
        }

        private void OnGoldChanged(int gold)
        {
            RefreshAll();
        }

        private void RefreshAll()
        {
            var skills = _playerSkillService.EquippedSkills;
            for (int i = 0; i < _skillButtons.Length; i++)
            {
                if (_skillButtons[i] == null) continue;
                var skill = skills[i];
                bool canActivate = _playerSkillService.CanActivateSkill(i);
                _skillButtons[i].Setup(skill, canActivate);
            }
        }

        private void ShowTargetingPrompt(SkillTargetMode mode)
        {
            if (_targetingPrompt == null) return;
            _targetingPrompt.gameObject.SetActive(true);
            _targetingPrompt.text = mode == SkillTargetMode.TwoTowers
                ? "Select two towers to swap"
                : "Select a tower";
        }

        private void HideTargetingPrompt()
        {
            if (_targetingPrompt != null)
                _targetingPrompt.gameObject.SetActive(false);
        }
    }
}
