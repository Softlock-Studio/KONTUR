using UnityEngine;

namespace Game.House.Presentation
{
    [RequireComponent(typeof(Zone))]
    public class ZoneLightView : MonoBehaviour
    {
        [SerializeField] private GameObject _dayVolume;
        [SerializeField] private GameObject _nightVolume;

        private Zone _zone;

        private void Start()
        {
            _zone = GetComponent<Zone>();

            _zone.LightChanged += OnLightChanged;
        }

        private void OnLightChanged()
        {
            _dayVolume.SetActive(_zone.HasLight);
            _nightVolume.SetActive(!_zone.HasLight);
        }
    }
}