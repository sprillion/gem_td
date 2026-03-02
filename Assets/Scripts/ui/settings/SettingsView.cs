using UnityEngine;
using UnityEngine.UI;

namespace ui.settings
{
    public class SettingsView : Popup
    {
        [SerializeField] private Button _backButton;

        private void Awake()
        {
            _backButton.onClick.AddListener(Back);
        }
    }
}