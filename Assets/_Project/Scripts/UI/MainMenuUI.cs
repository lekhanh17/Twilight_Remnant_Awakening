using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace TwilightRemnant
{
    /// <summary>
    /// Màn hình chính. New Game / Load / Settings / Thoát.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private TextMeshProUGUI noSaveMessageText;

        [SerializeField] private string gameplaySceneName = "Bootstrap";

        private const string PendingLoadKey = "TR_PendingLoad";

        private void Awake()
        {
            newGameButton.onClick.AddListener(OnNewGameClicked);
            loadButton.onClick.AddListener(OnLoadClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            quitButton.onClick.AddListener(OnQuitClicked);

            loadButton.interactable = SaveSystem.SaveFileExists();

            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (noSaveMessageText != null) noSaveMessageText.gameObject.SetActive(false);
        }

        private void OnNewGameClicked()
        {
            PlayerPrefs.DeleteKey(PendingLoadKey);
            SceneManager.LoadScene(gameplaySceneName);
        }

        private void OnLoadClicked()
        {
            if (!SaveSystem.SaveFileExists())
            {
                if (noSaveMessageText != null)
                    noSaveMessageText.gameObject.SetActive(true);
                return;
            }

            PlayerPrefs.SetInt(PendingLoadKey, 1);
            SceneManager.LoadScene(gameplaySceneName);
        }

        private void OnSettingsClicked()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(!settingsPanel.activeSelf);
        }

        private void OnQuitClicked()
        {
            Application.Quit();
        }
    }
}