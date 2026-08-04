using Game.Audio;
using Game.Bootstrap;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class InstructionsView : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Button _startButton;
    [SerializeField] private SfxCue _soundCue;

    public Button GetStartButton() => _startButton;
    private const string ANIM_TRIGGER = "Appear";


    public void ShowInstruction()
    {
        gameObject.SetActive(true);
        _animator.SetTrigger(ANIM_TRIGGER);
    }

    public void PlaySFX()
    {
        IAudioService audioService = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IAudioService>();
        audioService.PlayUiSfx(_soundCue);
    }
}
