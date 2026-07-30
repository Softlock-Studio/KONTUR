using CameraSystem.Rendering;
using Game.House;
using UnityEngine;

namespace CameraSystem
{
    [RequireComponent(typeof(Collider))]
    public class GameCamera : MonoBehaviour
    {
        [SerializeField] private Camera _correspondingCamera;
        [SerializeField] private string _localisationKey;
        [SerializeField] private Zone _observedZone;
        private int _cameraID;

        private void Awake()
        {
            _cameraID = gameObject.GetInstanceID();
        }
        public int GetCameraID() => _cameraID;
        public string GetLocalisationKey() => _localisationKey;
        public Zone GetObservedZone() => _observedZone;
        public void TurnOffCamera()
        {
            _correspondingCamera.targetTexture = null;
            _correspondingCamera.enabled = false;
        }

        public void TurnOnCamera(RenderTexture cameraTexture)
        {
            _correspondingCamera.targetTexture = cameraTexture;
            _correspondingCamera.enabled = true;
        }

        public bool TrySetNoiseBlend(float blend)
        {
            if (_correspondingCamera.TryGetComponent(out FisheyeLensMarker marker))
            {
                marker.SetNoiseBlend(blend);
                return true;
            }

            return false;
        }

        public float GetNoiseBlend()
        {
            return _correspondingCamera.TryGetComponent(out FisheyeLensMarker marker) ? marker.NoiseBlend : 0f;
        }
    }
}
