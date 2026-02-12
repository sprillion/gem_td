using TMPro;
using towers.abilities.effects;
using UnityEngine;
using UnityEngine.UI;

namespace ui.selection
{
    public class EffectItemUI : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _durationText;
        [SerializeField] private TMP_Text _descriptionText;

        public void Setup(Effect effect)
        {
            if (effect == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            if (_iconImage != null && effect.Icon != null)
            {
                _iconImage.sprite = effect.Icon;
                _iconImage.enabled = true;
            }
            else if (_iconImage != null)
            {
                _iconImage.enabled = false;
            }

            if (_nameText != null)
            {
                _nameText.text = effect.DisplayName ?? "Unknown Effect";
            }

            if (_durationText != null)
            {
                _durationText.text = $"{effect.RemainingDuration:F1}s";
            }

            if (_descriptionText != null)
            {
                _descriptionText.text = effect.Description ?? "";
            }
        }
    }
}
