using Game.Bootstrap;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Localization
{
    public sealed class LocalizedTextTMP : MonoBehaviour
    {
        [SerializeField] private string localizationKey;

        private ILocalizationService localization;
        private TMP_Text label;

        private void Awake()
        {
            label = GetComponent<TMP_Text>();

            if (label == null)
                Debug.LogError($"[{name}] LocalizedTextTMP requires a TextMeshProUGUI or TextMeshPro component.", this);
        }

        // Resolved here rather than via [Inject]/Auto Inject Game Objects — GameLifetimeScope is
        // now a persistent root scope spawned outside any scene (see VContainerSettings), so it
        // can't hold Inspector references to scene-local objects like this one. Same pattern as
        // MainMenuUI (see Docs/agents/systems/ui.md).
        private void Start()
        {
            localization = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<ILocalizationService>();
            localization.LanguageChanged += Localize;
            Localize();
        }

        private void OnDestroy()
        {
            if (localization != null) localization.LanguageChanged -= Localize;
        }

        private void Localize()
        {
            label.text = localization.Localize(localizationKey);
        }

        // For dynamically-bound instances (e.g. one per ResourceType, spawned at runtime) where
        // the key isn't known until bind time. Guarded because Start (where localization/label
        // get set) may not have run yet if this is called in the same frame as Instantiate.
        public void SetKey(string key)
        {
            localizationKey = key;
            if (label != null && localization != null) Localize();
        }
    }
}
