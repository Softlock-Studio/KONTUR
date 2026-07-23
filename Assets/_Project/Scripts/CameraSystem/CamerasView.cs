using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CameraSystem
{
    public class CamerasView : MonoBehaviour, ICamerasView
    {
        [SerializeField] private TextMeshProUGUI _cameraLabel;
        [SerializeField] private List<GameCamera> _camerasList;
        [SerializeField] private RenderTexture _renderTexture;

        public event Action<int> OnHandleClick;

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
                    _cameraLabel.text = cam.GetLocalisationKey();
                }
                else
                { 
                    cam.TurnOffCamera();
                }
            }
        }
    }
}