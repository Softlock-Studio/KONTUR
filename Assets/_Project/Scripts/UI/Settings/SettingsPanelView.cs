using Game.Audio;
using Game.Bootstrap;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Game.UI.Settings
{
    public sealed class SettingsPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Button openButton;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        private IAudioService audioService;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            if (openButton != null) openButton.onClick.AddListener(TogglePanel);
        }

        // IAudioService is game-wide (GameLifetimeScope), not mission-scoped — resolve from the
        // persistent root scope, not MissionScope.
        private void Start()
        {
            audioService = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IAudioService>();

            if (masterSlider != null)
            {
                masterSlider.SetValueWithoutNotify(audioService.MasterVolume);
                masterSlider.onValueChanged.AddListener(audioService.SetMasterVolume);
            }

            if (musicSlider != null)
            {
                musicSlider.SetValueWithoutNotify(audioService.MusicVolume);
                musicSlider.onValueChanged.AddListener(audioService.SetMusicVolume);
            }

            if (sfxSlider != null)
            {
                sfxSlider.SetValueWithoutNotify(audioService.SfxVolume);
                sfxSlider.onValueChanged.AddListener(audioService.SetSfxVolume);
            }
        }

        private void TogglePanel()
        {
            if (panelRoot != null) panelRoot.SetActive(!panelRoot.activeSelf);
        }
    }
}
