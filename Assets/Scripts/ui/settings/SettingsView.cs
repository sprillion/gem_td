using UnityEngine;
using UnityEngine.UI;

namespace ui.settings
{
    public class SettingsView : Popup
    {
        [SerializeField] private Button _backButton;

        public override void Initialize()
        {
            _backButton.onClick.AddListener(Back);
        }
    }
}