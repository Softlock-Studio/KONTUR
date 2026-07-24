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

        // Cycles to the other language — only 2 exist today, so there's nothing to enumerate.
        // Wire this straight to a future single-button language toggle; SetLanguage is still there
        // for a proper EN/RU picker later.
        void ToggleLanguage();
    }
}
