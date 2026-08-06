using Game.Audio;
using Game.Bootstrap;
using Game.Localization;
using Loader.SceneController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class ReportView : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SfxCue _paaperCue;
    [Header("Worker letter")]
    [SerializeField] private TextMeshProUGUI _maxInfectionText;
    [SerializeField] private TextMeshProUGUI _deadEmployeesText;
    [SerializeField] private LocalizedTextTMP _extraText;

    [Header("KONTUR letter")]
    [SerializeField] private LocalizedTextTMP _konturText;

    [Header("Button")]
    [SerializeField] private Button _button;
    [SerializeField] private LocalizedTextTMP _buttonText;

    private const string ANIM_TRIGGER = "ReportAppear";
    private SceneController _sceneController;
    private IAudioService _audioService;

    private const LevelType _lastLevel = LevelType.Level3;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hasClearedLevel">if set to true, button changes to "continue", otherwise it changes to "replay day"</param>
    public void ShowReport(bool hasClearedLevel, int maxInfection, int numOfDeadEmployees)
    {
        _sceneController = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<SceneController>();
        _audioService = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IAudioService>();

        _maxInfectionText.text = maxInfection.ToString();
        _deadEmployeesText.text = numOfDeadEmployees.ToString();
        _extraText.SetKey("Report.Worker.Extra." + ((int)_sceneController.GetCurrentLevelType()).ToString());

        _konturText.SetKey("Report.Kontur." + (hasClearedLevel ? "SuccessText." : "FailText.") + ((int)_sceneController.GetCurrentLevelType()).ToString());

        if (hasClearedLevel)
        {
            _buttonText.SetKey("Report.ContinueButton");
            if (_sceneController.GetCurrentLevelType() == _lastLevel)
                _button.onClick.AddListener(() => { _sceneController.LevelLoad(LevelType.MainMenu); });
            else
                _button.onClick.AddListener(() => { _sceneController.LevelLoad(_sceneController.GetCurrentLevelType() + 1); });
        }
        else
        {
            _buttonText.SetKey("Report.ReplayButton");
            _button.onClick.AddListener(() => { _sceneController.LevelLoad(_sceneController.GetCurrentLevelType()); });
        }

        gameObject.SetActive(true);
        _animator.SetTrigger(ANIM_TRIGGER);
    }

    public void PlayPaperSound()
    {
        _audioService.PlayUiSfx(_paaperCue);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }
}
