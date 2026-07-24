using System;
using Assets.SimpleLocalization.Scripts;
using UnityEngine;
using VContainer.Unity;

namespace Game.Localization
{
    public sealed class LocalizationService : ILocalizationService, IStartable, IDisposable
    {
        private const string LanguagePrefKey = "Localization.Language";
        private const string English = "English";
        private const string Russian = "Russian";

        public event Action LanguageChanged;

        public string CurrentLanguage => LocalizationManager.Language;

        public void Start()
        {
            LocalizationManager.Read();
            LocalizationManager.OnLocalizationChanged += HandleLocalizationChanged;

            SetLanguage(PlayerPrefs.GetString(LanguagePrefKey, DetectSystemLanguage()));
        }

        public string Localize(string key) => LocalizationManager.Localize(key);

        public string Localize(string key, params object[] args) => LocalizationManager.Localize(key, args);

        public void SetLanguage(string language)
        {
            LocalizationManager.Language = language;
            PlayerPrefs.SetString(LanguagePrefKey, language);
            PlayerPrefs.Save();
        }

        public void ToggleLanguage() => SetLanguage(CurrentLanguage == Russian ? English : Russian);

        public void Dispose()
        {
            LocalizationManager.OnLocalizationChanged -= HandleLocalizationChanged;
        }

        private void HandleLocalizationChanged() => LanguageChanged?.Invoke();

        private static string DetectSystemLanguage()
        {
            return Application.systemLanguage switch
            {
                SystemLanguage.Russian => Russian,
                _ => English,
            };
        }
    }
}
