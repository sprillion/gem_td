using System;
using skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ui.skills
{
    public class PlayerSkillButton : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _cooldownOverlay;
        [SerializeField] private TextMeshProUGUI _cooldownText;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Button _button;
        [SerializeField] private Image _disabledOverlay;

        public event Action OnClicked;

        private PlayerSkillInstance _skill;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnClicked?.Invoke());
        }

        public void Setup(PlayerSkillInstance skill, bool canActivate)
        {
            _skill = skill;

            if (skill == null || skill.Data == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            if (_iconImage != null && skill.Data.Icon != null)
                _iconImage.sprite = skill.Data.Icon;

            if (_costText != null)
                _costText.text = skill.GoldCost.ToString();

            SetInteractable(canActivate);
            UpdateCooldown(skill.CooldownRemaining, skill.Cooldown);
        }

        public void UpdateCooldown(float remaining, float total)
        {
            bool onCooldown = remaining > 0f;

            if (_cooldownOverlay != null)
            {
                _cooldownOverlay.gameObject.SetActive(onCooldown);
                if (onCooldown && total > 0f)
                    _cooldownOverlay.fillAmount = remaining / total;
            }

            if (_cooldownText != null)
            {
                _cooldownText.gameObject.SetActive(onCooldown);
                if (onCooldown)
                    _cooldownText.text = Mathf.CeilToInt(remaining).ToString();
            }
        }

        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
            if (_disabledOverlay != null)
                _disabledOverlay.gameObject.SetActive(!interactable);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}
