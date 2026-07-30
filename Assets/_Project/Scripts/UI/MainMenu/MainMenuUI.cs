using Game.Bootstrap;
using Game.UI.Settings;
using Loader.SceneController;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Game.UI.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private SettingsPanelView _settingsPanelView;
        [SerializeField] private YesNoPopUp _startGamePopUpView;
        [SerializeField] private GameObject _mainMenuButtonsObject;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;

        private SceneController _sceneController;

        private void Start()
        {
            var scope = LifetimeScope.Find<GameLifetimeScope>();

            _sceneController = scope.Container.Resolve<SceneController>();

            //TODO connect saving system when ready
            _continueButton.interactable = /*has found existing save file */ false;

            _settingsPanelView.GetBackButton().onClick.AddListener(OpenMainMenu);
            _continueButton.onClick.AddListener(ContinueGame);
            _newGameButton.onClick.AddListener(StartNewGame);
            _settingsButton.onClick.AddListener(OpenSettingsMenu);
            _quitButton.onClick.AddListener(QuitGame);
            _startGamePopUpView.GetYesButton().onClick.AddListener(LoadNewGame);
            _startGamePopUpView.GetNoButton().onClick.AddListener(ClosePopUp);
        }

        private void ContinueGame()
        {
            //TODO connect saving system when ready

            _sceneController.LevelLoad(LevelType.Level1);
        }

        private void StartNewGame()
        {
            //TODO connect saving system when ready
            if (/**/ true)
            {
                OpenPopUp();
            }
            else
            {
                LoadNewGame();
            }
        }

        private void LoadNewGame()
        {
            _sceneController.LevelLoad(LevelType.Level1);
        }

        private void OpenPopUp()
        {
            _startGamePopUpView.gameObject.SetActive(true);
        }

        private void ClosePopUp()
        {
            _startGamePopUpView.gameObject.SetActive(false);
        }

        private void OpenSettingsMenu()
        {
            _settingsPanelView.gameObject.SetActive(true);
            _mainMenuButtonsObject.gameObject.SetActive(false);
        }

        private void OpenMainMenu()
        {
            _settingsPanelView.gameObject.SetActive(false);
            _mainMenuButtonsObject.gameObject.SetActive(true);
        }

        private void QuitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }

        private void OnDestroy()
        {
            _settingsPanelView.GetBackButton().onClick.RemoveListener(OpenMainMenu);
            _continueButton.onClick.RemoveListener(ContinueGame);
            _newGameButton.onClick.RemoveListener(StartNewGame);
            _settingsButton.onClick.RemoveListener(OpenSettingsMenu);
            _quitButton.onClick.RemoveListener(QuitGame);
            _startGamePopUpView.GetYesButton();
            _startGamePopUpView.GetNoButton();
        }
    }
}