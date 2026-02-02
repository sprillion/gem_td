using UnityEngine;
using UnityEngine.UI;

namespace ui.menu
{
    public class PlayView : Popup
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _backButton;

        public override void Initialize()
        {
            _continueButton.onClick.AddListener(Continue);
            _newGameButton.onClick.AddListener(StartNewGame);
            _backButton.onClick.AddListener(Back);
        }

        private void Continue()
        {
            
        }

        private void StartNewGame()
        {
            
        }
    }
}