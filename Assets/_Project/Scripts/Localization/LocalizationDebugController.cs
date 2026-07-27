using Game.Bootstrap;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace Game.Localization
{
    public sealed class LocalizationDebugController : MonoBehaviour
    {
        [SerializeField] private bool debugEnabled = true;
        [SerializeField] private Key toggleKey = Key.L;

        private ILocalizationService localization;

        private void Start()
        {
            localization = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<ILocalizationService>();
        }

        private void Update()
        {
            if (!debugEnabled || localization == null) return;

            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard[toggleKey].wasPressedThisFrame) localization.ToggleLanguage();
        }
    }
}
