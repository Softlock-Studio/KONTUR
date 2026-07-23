using System;
using System.Collections.Generic;
using Game.Bootstrap;
using Game.Localization;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CameraSystem
{
    public class CamerasView : MonoBehaviour, ICamerasView
    {
        [SerializeField] private TextMeshProUGUI _cameraLabel;
        [SerializeField] private List<GameCamera> _camerasList;
        [SerializeField] private RenderTexture _renderTexture;

        private ILocalizationService localization;

        public event Action<int> OnHandleClick;

        // ILocalizationService is game-wide (GameLifetimeScope), not mission-scoped — same
        // resolve pattern as SettingsPanelView/EmployeeSlotView.
        private ILocalizationService Localization =>
            localization ??= LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<ILocalizationService>();

        // Auto-populated so every GameCamera in the scene is found, instead of relying on a
        // manually-maintained (and easy to leave incomplete) serialized list.
        private void Awake()
        {
            _camerasList = new List<GameCamera>(FindObjectsByType<GameCamera>(FindObjectsSortMode.None));
        }

        public List<GameCamera> GetCameraList() => _camerasList;

        public void HandleClick(int cameraID)
        {
            OnHandleClick?.Invoke(cameraID);
        }

        public void SelectCamera(int cameraID)
        {
            foreach (GameCamera cam in _camerasList)
            {
                if (cam.GetCameraID() == cameraID)
                {
                    cam.TurnOnCamera(_renderTexture);
                    // GetLocalisationKey() is a lookup key, not display text — it was being shown
                    // verbatim (never routed through ILocalizationService), so switching language
                    // never affected this label.
                    _cameraLabel.text = Localization.Localize(cam.GetLocalisationKey());
                }
                else
                {
                    cam.TurnOffCamera();
                }
            }
        }
    }
}