using Game.Audio;
using Game.Bootstrap;
using Game.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Game.UI.Settings
{
    public sealed class SettingsPanelView : MonoBehaviour
    {
        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private TMP_Dropdown _languageDropdown;
        [SerializeField] private Button _backButton;

        public Button GetBackButton() => _backButton;

        private IAudioService _audioService;
        private ILocalizationService _localization;

        private void Start()
        {
            _audioService = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IAudioService>();
            _localization = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<ILocalizationService>();

            if (_masterSlider == null)
                Debug.LogError($"Master Slider wasn't set in {gameObject.name}");

            if (_musicSlider == null)
                Debug.LogError($"Music Slider wasn't set in {gameObject.name}");

            if (_sfxSlider == null)
                Debug.LogError($"SFX Slider wasn't set in {gameObject.name}");

            if (_languageDropdown == null)
                Debug.LogError($"Language Dropdown wasn't set in {gameObject.name}");

            if (_backButton == null)
                Debug.LogError($"Back button wasn't set in {gameObject.name}");

            _masterSlider.SetValueWithoutNotify(_audioService.MasterVolume);
            _masterSlider.onValueChanged.AddListener(_audioService.SetMasterVolume);

            _musicSlider.SetValueWithoutNotify(_audioService.MusicVolume);
            _musicSlider.onValueChanged.AddListener(_audioService.SetMusicVolume);

            _sfxSlider.SetValueWithoutNotify(_audioService.SfxVolume);
            _sfxSlider.onValueChanged.AddListener(_audioService.SetSfxVolume);

            _languageDropdown.onValueChanged.AddListener(ChangeLanguage);

            _languageDropdown.value = _localization.CurrentLanguage == "English" ? 0 : 1;
        }

        private void ChangeLanguage(int num)
        {
            switch (num)
            {
                case 0:
                    _localization.SetLanguage("English");
                    break;
                case 1:
                    _localization.SetLanguage("Russian");
                    break;
            }
        }

        private void OnDestroy()
        {
            _masterSlider.onValueChanged.RemoveListener(_audioService.SetMasterVolume);
            _musicSlider.onValueChanged.RemoveListener(_audioService.SetMusicVolume);
            _sfxSlider.onValueChanged.RemoveListener(_audioService.SetSfxVolume);
            _languageDropdown.onValueChanged.RemoveListener(ChangeLanguage);
        }
    }
}
