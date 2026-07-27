using Game.Bootstrap;
using Game.Input;
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
        private IInputService input;

        private void Start()
        {
            IObjectResolver container = LifetimeScope.Find<GameLifetimeScope>().Container;
            localization = container.Resolve<ILocalizationService>();
            input = container.Resolve<IInputService>();
        }

        private void Update()
        {
            if (!debugEnabled || localization == null || input == null) return;

            if (input.WasKeyPressedThisFrame(toggleKey)) localization.ToggleLanguage();
        }
    }
}
