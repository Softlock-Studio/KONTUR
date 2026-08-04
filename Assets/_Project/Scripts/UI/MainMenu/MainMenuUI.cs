using Game.Bootstrap;
using Game.UI.Settings;
using Loader.SceneController;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Game.UI.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private SettingsPanelView _settingsPanelView;
        [SerializeField] private InstructionsView _instructionsView;
        [SerializeField] private Animator _menuAnimator;
        [SerializeField] private YesNoPopUp _startGamePopUpView;
        [SerializeField] private GameObject _mainMenuButtonsObject;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;

        private const string SETTINGS_ANIM_TRIGGER = "SETTINGS_APPEAR";
        private const string MENU_ANIM_TRIGGER = "MENU_APPEAR";
        private const string CLOSE_SETTINGS_STATE = "ButtonsPanelLeftToRight";
        private const string CLOSE_MENU_STATE = "ButtonsPanelRightToLeft";

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

            _instructionsView.GetStartButton().onClick.AddListener(() => { _sceneController.LevelLoad(LevelType.Level1); });
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
            _instructionsView.ShowInstruction();
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
            StartCoroutine(PlayOpenSettingsAnimation());
        }

        private void OpenMainMenu()
        {
            StartCoroutine(PlayOpenMenuAnimation());
        }

        private IEnumerator PlayOpenMenuAnimation()
        {
            _menuAnimator.SetTrigger(MENU_ANIM_TRIGGER);
            _mainMenuButtonsObject.gameObject.SetActive(true);
            yield return new WaitUntil(delegate { return _menuAnimator.GetCurrentAnimatorStateInfo(0).IsName(CLOSE_SETTINGS_STATE); });
            _settingsPanelView.gameObject.SetActive(false);
        }

        private IEnumerator PlayOpenSettingsAnimation()
        {
            _menuAnimator.SetTrigger(SETTINGS_ANIM_TRIGGER);
            _settingsPanelView.gameObject.SetActive(true);
            yield return new WaitUntil(delegate { return _menuAnimator.GetCurrentAnimatorStateInfo(0).IsName(CLOSE_MENU_STATE); });
            _mainMenuButtonsObject.gameObject.SetActive(false);
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
            _instructionsView.GetStartButton().onClick.RemoveAllListeners();
        }
    }
}