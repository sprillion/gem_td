using ui.settings;
using UnityEngine;
using UnityEngine.UI;

namespace ui.menu
{
    public class MenuView : Popup
    {
        [SerializeField] private PlayView _playView;
        [SerializeField] private SettingsView _settingsView;
        
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;

        public override void Initialize()
        {
            _playView.Initialize();
            _settingsView.Initialize();
            
            _playButton.onClick.AddListener(Play);
            _settingsButton.onClick.AddListener(OpenSettings);
            _exitButton.onClick.AddListener(Exit);
        }

        private void Play()
        {
            _playView.Show(this);
        }

        private void OpenSettings()
        {
            _settingsView.Show(this);
        }

        private void Exit()
        {
            Application.Quit();
        }
    }
}