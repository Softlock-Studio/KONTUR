using Game.Bootstrap;
using Loader.SceneController;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI.Settings
{
    public class PauseMenuView : MonoBehaviour
    {
        [SerializeField] private SettingsPanelView _settingsPanelView;
        [SerializeField] private PauseMenuButtonsView _pauseMenuButtonsView;
        [SerializeField] private YesNoPopUp _quitPopUp;

        private SceneController _sceneController;

        private void Start()
        {
            var scope = LifetimeScope.Find<GameLifetimeScope>();
            _sceneController = scope.Container.Resolve<SceneController>();

            if (_settingsPanelView == null)
            {
                Debug.LogError($"SettingsPanelView wasn't set in {gameObject.name}");
            }
            if (_pauseMenuButtonsView == null)
            {
                Debug.LogError($"PauseMenuButtonsView wasn't set in {gameObject.name}");
            }
            if (_quitPopUp == null)
            {
                Debug.LogError($"QuitPopUp wasn't set in {gameObject.name}");
            }
        }

        public void ShowPauseMenu()
        { 
            gameObject.SetActive(true);
        }

        private void ResumeGame()
        {
            _settingsPanelView.gameObject.SetActive(false);
            _pauseMenuButtonsView.gameObject.SetActive(true);
            _quitPopUp.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        private void SwitchToSettings()
        {
            _pauseMenuButtonsView.gameObject.SetActive(false);
            _settingsPanelView.gameObject.SetActive(true);
        }

        private void SwitchToPauseMenu()
        {
            _pauseMenuButtonsView.gameObject.SetActive(true);
            _settingsPanelView.gameObject.SetActive(false);
        }

        private void QuitToMainMenu()
        {
            _sceneController.LevelLoad(LevelType.MainMenu);
        }

        private void ShowQuitPopUp()
        {
            _quitPopUp.gameObject.SetActive(true);
        }

        private void CloseQuitPopUp()
        {
            _quitPopUp.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _pauseMenuButtonsView.GetResumeButton().onClick.AddListener(ResumeGame);
            _pauseMenuButtonsView.GetSettingsButton().onClick.AddListener(SwitchToSettings);
            _pauseMenuButtonsView.GetBackToMenuButton().onClick.AddListener(ShowQuitPopUp);
            _settingsPanelView.GetBackButton().onClick.AddListener(SwitchToPauseMenu);

            _quitPopUp.GetYesButton().onClick.AddListener(QuitToMainMenu);
            _quitPopUp.GetNoButton().onClick.AddListener(CloseQuitPopUp);
        }

        private void OnDisable()
        {
            _pauseMenuButtonsView.GetResumeButton().onClick.RemoveListener(ResumeGame);
            _pauseMenuButtonsView.GetSettingsButton().onClick.RemoveListener(SwitchToSettings);
            _pauseMenuButtonsView.GetBackToMenuButton().onClick.RemoveListener(ShowQuitPopUp);
            _settingsPanelView.GetBackButton().onClick.RemoveListener(SwitchToPauseMenu);

            _quitPopUp.GetYesButton().onClick.RemoveListener(QuitToMainMenu);
            _quitPopUp.GetNoButton().onClick.RemoveListener(CloseQuitPopUp);
        }
    }
}