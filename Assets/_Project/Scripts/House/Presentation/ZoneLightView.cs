using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Game.House.Presentation
{
    [RequireComponent(typeof(Zone))]
    public class ZoneLightView : MonoBehaviour
    {
        [SerializeField] private Camera _cameraRoom;
        [Space]
        [SerializeField] private GameObject _dayVolume;
        [SerializeField] private GameObject _nightVolume;
        [SerializeField] private GameObject _lightObject;
        [Space]
        [SerializeField] private AnimationCurve _curveParticle;
        [SerializeField] private float _minValue = 5;
        [SerializeField] private float _maxValue = 20;
        [SerializeField] private ParticleSystem _particle;

        private Zone _zone;

        private void OnValidate()
        {
            _curveParticle = new AnimationCurve();
            _curveParticle.AddKey(0.0f, _minValue);
            _curveParticle.AddKey(1.0f, _maxValue);
        }

        private void OnDisable()
        {
            _zone.LightChanged -= OnLightChanged;
            _zone.InfectionChanged -= OnInfectionChanged;
        }

        private void Start()
        {
            _zone = GetComponent<Zone>();

            _zone.LightChanged += OnLightChanged;
            _zone.InfectionChanged += OnInfectionChanged;

            var em = _particle.emission;
            em.rateOverTime = _minValue;
            _particle.Play();
        }

        private void Update()
        {
            if(_cameraRoom.enabled)
                SetActiveObject(true);
            else
                SetActiveObject(false);
        }

        private void SetActiveObject(bool isActive)
        {
            if(_lightObject != null)
                _lightObject.SetActive(isActive);

            if (_particle != null)
            {
                _particle.gameObject.SetActive(isActive);
                _particle.Play();
            }
        }

        private void OnLightChanged()
        {
            _dayVolume.SetActive(_zone.HasLight);
            _nightVolume.SetActive(!_zone.HasLight);
        }

        private void OnInfectionChanged(float infection)
        {
            if (_particle == null)
                return;

            var em = _particle.emission;
            em.rateOverTime = _curveParticle.Evaluate(infection);
        }
    }
}