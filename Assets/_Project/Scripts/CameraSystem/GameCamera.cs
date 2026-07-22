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
        }

        public void TurnOnCamera(RenderTexture cameraTexture)
        {
            _correspondingCamera.targetTexture = cameraTexture;
        }
    }
}
