using ui;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ui.menu
{
    public class PlayView : Popup
    {
        private const string SAVE_KEY = "GemTD_GameSave";

        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _backButton;

        public override void Initialize()
        {
            _continueButton.onClick.AddListener(Continue);
            _newGameButton.onClick.AddListener(StartNewGame);
            _backButton.onClick.AddListener(Back);

            // Enable/disable Continue button based on save data existence
            UpdateContinueButton();
        }

        public override void Show(Popup parent = null)
        {
            base.Show(parent);

            // Update button state when showing the view
            UpdateContinueButton();
        }

        private void UpdateContinueButton()
        {
            bool hasSave = PlayerPrefs.HasKey(SAVE_KEY);
            _continueButton.interactable = hasSave;

            if (!hasSave)
            {
                Debug.Log("No save data found - Continue button disabled");
            }
        }

        private void Continue()
        {
            if (!PlayerPrefs.HasKey(SAVE_KEY))
            {
                Debug.LogWarning("No save data to continue from!");
                return;
            }

            Debug.Log("Loading saved game...");

            // Find and destroy the UI root (which has DontDestroyOnLoad)
            var uiManager = GetComponentInParent<UiManager>();
            if (uiManager != null)
            {
                Destroy(uiManager.gameObject);
            }

            // Load Game scene - the save will auto-load in SceneInstaller
            SceneManager.LoadScene("Game");
        }

        private void StartNewGame()
        {
            // Delete existing save to start fresh
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                PlayerPrefs.DeleteKey(SAVE_KEY);
                PlayerPrefs.Save();
                Debug.Log("Deleted existing save - starting new game");
            }

            // Find and destroy the UI root (which has DontDestroyOnLoad)
            var uiManager = GetComponentInParent<UiManager>();
            if (uiManager != null)
            {
                Destroy(uiManager.gameObject);
            }

            SceneManager.LoadScene("Game");
        }
    }
}
