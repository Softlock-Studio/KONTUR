using TMPro;
using UnityEngine;
using VContainer;

namespace Game.Localization
{
    public sealed class LocalizedTextTMP : MonoBehaviour
    {
        [SerializeField] private string localizationKey;

        private ILocalizationService localization;
        private TMP_Text label;

        [Inject]
        public void Construct(ILocalizationService localization)
        {
            this.localization = localization;
        }

        private void Awake()
        {
            label = GetComponent<TMP_Text>();

            if (label == null)
                Debug.LogError($"[{name}] LocalizedTextTMP requires a TextMeshProUGUI or TextMeshPro component.", this);
        }

        private void Start()
        {
            localization.LanguageChanged += Localize;
            Localize();
        }

        private void OnDestroy()
        {
            localization.LanguageChanged -= Localize;
        }

        private void Localize()
        {
            label.text = localization.Localize(localizationKey);
        }

        // For dynamically-bound instances (e.g. one per ResourceType, spawned at runtime) where
        // the key isn't known until bind time. Safe to call right after
        // IObjectResolver.Instantiate — Awake/[Inject] run synchronously as part of instantiation,
        // only Start (which subscribes to LanguageChanged) is deferred.
        public void SetKey(string key)
        {
            localizationKey = key;
            if (label != null && localization != null) Localize();
        }
    }
}
