using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infection
{
    public class InfectionGroup : MonoBehaviour
    {
        [SerializeField] Slider _infectionSlider;
        [SerializeField] TextMeshProUGUI _infectionRangeLabel;
        [SerializeField] TextMeshProUGUI _infectionPercentLabel;
        [SerializeField] Image _leftBorder;
        [SerializeField] Image _rightBorder;
        [SerializeField] RectTransform _leftBracket;
        [SerializeField] RectTransform _rightBracket;
        private float _minRangeValue;
        private float _maxRangeValue;
        private float _rectWidth;

        public Slider GetSlider() => _infectionSlider;

        public void SetInfectionPercent(float value)
        {
            _infectionSlider.value = value;
            _infectionPercentLabel.text = $"{value * 100f:F0}%";
        }

        public void SetInfectionRangeValues(float min, float max)
        {
            _minRangeValue = min;
            _maxRangeValue = max;
            _leftBorder.fillAmount = _minRangeValue;
            _rightBorder.fillAmount = 1 - _maxRangeValue;

            //setting brackets
            _rectWidth = _leftBracket.offsetMin.x * 2;

            float _minPositionInPixels = _rectWidth * _minRangeValue;
            float _maxPositionInPixels = _rectWidth * _maxRangeValue;

            _leftBracket.offsetMin = new Vector2(_minPositionInPixels, _leftBracket.offsetMin.y);
            _leftBracket.offsetMax = new Vector2(_minPositionInPixels - _rectWidth, _leftBracket.offsetMax.y);

            _rightBracket.offsetMin = new Vector2(_maxPositionInPixels, _leftBracket.offsetMin.y);
            _rightBracket.offsetMax = new Vector2(_maxPositionInPixels - _rectWidth, _leftBracket.offsetMax.y);

            _infectionRangeLabel.text =  $"{min * 100:F0}-{max * 100:F0}%";        
        }

        private void Awake()
        {
            if (_infectionSlider == null)
            {
                Debug.LogError($"Slider wasn't set for {gameObject.name}");
            }
        }
    }
}