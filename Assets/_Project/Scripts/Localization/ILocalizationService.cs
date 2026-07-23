using System;

namespace Game.Localization
{
    public interface ILocalizationService
    {
        string CurrentLanguage { get; }

        event Action LanguageChanged;

        string Localize(string key);

        string Localize(string key, params object[] args);

        void SetLanguage(string language);
    }
}
