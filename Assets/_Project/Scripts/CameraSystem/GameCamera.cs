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
        private AudioListener _audioListener;

        private void Awake()
        {
            _cameraID = gameObject.GetInstanceID();
            // Auto-discovered, not serialized: every camera in AllCamera.prefab already has one
            // on the same GameObject as _correspondingCamera. Null-safe — a camera without one
            // just never produces spatial audio, nothing else breaks.
            _audioListener = _correspondingCamera.GetComponent<AudioListener>();
        }
        public int GetCameraID() => _cameraID;
        public string GetLocalisationKey() => _localisationKey;
        public Zone GetObservedZone() => _observedZone;
        public void TurnOffCamera()
        {
            _correspondingCamera.targetTexture = null;
            _correspondingCamera.enabled = false;
            if (_audioListener != null) _audioListener.enabled = false;
        }

        public void TurnOnCamera(RenderTexture cameraTexture)
        {
            _correspondingCamera.targetTexture = cameraTexture;
            _correspondingCamera.enabled = true;
            // Listener follows the selected camera, so world sounds (AudioService.CreateAttachedSource)
            // are heard as if standing where this camera is — see Game.Audio.AudioService.
            if (_audioListener != null) _audioListener.enabled = true;
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
