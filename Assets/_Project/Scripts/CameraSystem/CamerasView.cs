using System;
using System.Collections.Generic;
using CameraSystem.Rendering;
using Game.Audio;
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
        [SerializeField] private string _noSignalLocalisationKey;

        private GameObject _noSignalCameraObject;

        private ILocalizationService localization;
        private IAudioService audioService;

        public event Action<int> OnHandleClick;

        // ILocalizationService is game-wide (GameLifetimeScope), not mission-scoped — same
        // resolve pattern as SettingsPanelView/EmployeeSlotView.
        private ILocalizationService Localization =>
            localization ??= LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<ILocalizationService>();

        private IAudioService AudioService =>
            audioService ??= LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IAudioService>();

        // Auto-populated so every GameCamera in the scene is found, instead of relying on a
        // manually-maintained (and easy to leave incomplete) serialized list. Scene-filtered:
        // during a level transition the previous level is still loaded additively at this point
        // (see SceneController.LevelLoad), so an unfiltered FindObjectsByType would also pick up
        // the outgoing scene's cameras — which then get destroyed under us when it unloads.
        private void Awake()
        {
            _camerasList = new List<GameCamera>();
            foreach (GameCamera cam in FindObjectsByType<GameCamera>(FindObjectsSortMode.None))
                if (cam.gameObject.scene == gameObject.scene) _camerasList.Add(cam);
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
                    cam.TrySetNoiseBlend(GetNoiseBlend(cameraID));
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

        public void ShowNoSignal()
        {
            foreach (GameCamera cam in _camerasList)
            {
                cam.TurnOffCamera();
            }

            AudioService.ParkWorldListener();
            AudioService.StopUiLoop();

            if (_noSignalCameraObject == null)
            {
                _noSignalCameraObject = new GameObject("No Signal Camera (Dummy)");
                var camera = _noSignalCameraObject.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
                camera.cullingMask = 0;

                var marker = _noSignalCameraObject.AddComponent<FisheyeLensMarker>();
                marker.SetNoiseBlend(1f);

                camera.targetTexture = _renderTexture;
                camera.enabled = true;
            }

            _cameraLabel.text = Localization.Localize(_noSignalLocalisationKey);
        }

        public float GetNoiseBlend(int cameraID)
        {
            foreach (GameCamera cam in _camerasList)
            {
                if (cam.GetCameraID() == cameraID)
                {
                    return cam.GetNoiseBlend();
                }
            }

            return 0f;
        }
    }
}