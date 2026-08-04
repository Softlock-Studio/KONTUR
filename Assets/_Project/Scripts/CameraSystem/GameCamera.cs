using CameraSystem.Rendering;
using Game.Audio;
using Game.Bootstrap;
using Game.House;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CameraSystem
{
    [RequireComponent(typeof(Collider))]
    public class GameCamera : MonoBehaviour
    {
        [SerializeField] private Camera _correspondingCamera;
        [SerializeField] private string _localisationKey;
        [SerializeField] private Zone _observedZone;
        [SerializeField] private SpriteRenderer _iconRenderer;
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private SfxCue _clickCue;
        [SerializeField] private SfxCue _viewHumCue;

        private int _cameraID;
        private IAudioService _audioService;

        private void Awake()
        {
            _cameraID = gameObject.GetInstanceID();
        }
        private void Start()
        {
            _audioService = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IAudioService>();
        }

        public int GetCameraID() => _cameraID;
        public string GetLocalisationKey() => _localisationKey;
        public Zone GetObservedZone() => _observedZone;
        public void TurnOffCamera()
        {
            _correspondingCamera.targetTexture = null;
            _correspondingCamera.enabled = false;
            _iconRenderer.sprite = _defaultSprite;
        }

        public void TurnOnCamera(RenderTexture cameraTexture)
        {
            _correspondingCamera.targetTexture = cameraTexture;
            _correspondingCamera.enabled = true;
            _iconRenderer.sprite = _selectedSprite;
            _audioService.PlayUiSfx(_clickCue);
            _audioService.PlayUiLoop(_viewHumCue);
            _audioService.SetWorldListenerPosition(_correspondingCamera.transform.position, _correspondingCamera.transform.rotation);
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
