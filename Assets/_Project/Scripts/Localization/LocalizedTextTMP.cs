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
    }
}
