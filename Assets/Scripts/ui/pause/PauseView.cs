using ui.settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ui.pause
{
    public class PauseView : Popup
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitToMenuButton;
        [SerializeField] private SettingsView _settingsView;

        private void Awake()
        {
            _continueButton.onClick.AddListener(Continue);
            _settingsButton.onClick.AddListener(OpenSettings);
            _exitToMenuButton.onClick.AddListener(ExitToMenu);
        }

        private void OpenSettings()
        {
            _settingsView.Show(this);
        }

        public override void Show()
        {
            base.Show();
            Time.timeScale = 0f;
        }

        private void Continue()
        {
            Time.timeScale = 1f;
            Hide();
        }

        private void ExitToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Boot");
        }
    }
}
