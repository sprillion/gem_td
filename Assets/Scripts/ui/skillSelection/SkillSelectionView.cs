using System.Collections.Generic;
using infrastructure.services.gameStateService;
using infrastructure.services.playerSkillService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ui.skillSelection
{
    public class SkillSelectionView : Popup
    {
        [SerializeField] private SkillSelectionItem _skillItemPrefab;
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private TextMeshProUGUI _selectedCountText;

        private IPlayerSkillService _playerSkillService;
        private IGameStateService _gameStateService;

        private readonly List<SkillSelectionItem> _items = new List<SkillSelectionItem>();
        private readonly List<SkillSelectionItem> _selectedItems = new List<SkillSelectionItem>();

        private const int MaxSelected = 4;

        [Inject]
        private void Construct(IPlayerSkillService playerSkillService, IGameStateService gameStateService)
        {
            _playerSkillService = playerSkillService;
            _gameStateService = gameStateService;
        }

        private void Start()
        {
            PopulateSkills();
            UpdateUI();
            _confirmButton.onClick.AddListener(OnConfirmClicked);
        }

        private void PopulateSkills()
        {
            foreach (Transform child in _itemContainer)
                Destroy(child.gameObject);

            _items.Clear();
            _selectedItems.Clear();

            var availableSkills = _playerSkillService.AvailableSkills;
            foreach (var skillData in availableSkills)
            {
                var item = Instantiate(_skillItemPrefab, _itemContainer);
                if (item == null) continue;

                item.Setup(skillData);
                item.OnToggled += OnItemToggled;
                _items.Add(item);
            }
        }

        private void OnItemToggled(SkillSelectionItem item)
        {
            if (item.IsSelected)
            {
                item.SetSelected(false);
                _selectedItems.Remove(item);
            }
            else
            {
                if (_selectedItems.Count >= MaxSelected) return;
                item.SetSelected(true);
                _selectedItems.Add(item);
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_selectedCountText != null)
                _selectedCountText.text = $"{_selectedItems.Count}/{MaxSelected}";

            if (_confirmButton != null)
                _confirmButton.interactable = _selectedItems.Count == MaxSelected;
        }

        public void OnConfirmClicked()
        {
            if (_selectedItems.Count != MaxSelected) return;

            for (int i = 0; i < _selectedItems.Count; i++)
                _playerSkillService.EquipSkill(i, _selectedItems[i].Data, 0);

            _gameStateService.StartGame();
            Hide();
        }

        private void OnDestroy()
        {
            foreach (var item in _items)
            {
                if (item != null)
                    item.OnToggled -= OnItemToggled;
            }
        }
    }
}
