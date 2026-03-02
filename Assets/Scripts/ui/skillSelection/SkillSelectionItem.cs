using System;
using skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ui.skillSelection
{
    public class SkillSelectionItem : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private GameObject _selectedIndicator;
        [SerializeField] private Button _button;

        public event Action<SkillSelectionItem> OnToggled;
        public PlayerSkillData Data { get; private set; }
        public bool IsSelected { get; private set; }

        private void Awake()
        {
            _button.onClick.AddListener(Toggle);
        }

        public void Setup(PlayerSkillData data)
        {
            Data = data;
            IsSelected = false;

            if (_iconImage != null && data.Icon != null)
                _iconImage.sprite = data.Icon;

            if (_nameText != null)
                _nameText.text = data.SkillName;

            _selectedIndicator?.SetActive(false);
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            _selectedIndicator?.SetActive(selected);
        }

        private void Toggle()
        {
            OnToggled?.Invoke(this);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}
