using UnityEngine;
using UnityEngine.UI;

namespace Infection
{
    public class InfectionSlider : MonoBehaviour
    {
        [SerializeField] Slider _infectionSlider;
        [SerializeField] Image _leftBorder;
        [SerializeField] Image _rightBorder;
        [SerializeField] RectTransform _leftBracket;
        [SerializeField] RectTransform _rightBracket;
        private float _minRangeValue;
        private float _maxRangeValue;
        private float _rectWidth;

        public void SetRangeValues(float min, float max)
        {
            _minRangeValue = min;
            _maxRangeValue = max;
            _leftBorder.fillAmount = _minRangeValue;
            _rightBorder.fillAmount = 1 - _maxRangeValue;
        }

        private void Start()
        {
            _rectWidth = _leftBracket.offsetMin.x * 2;

            float _minPositionInPixels = _rectWidth * _minRangeValue;
            float _maxPositionInPixels = _rectWidth * _maxRangeValue;

            _leftBracket.offsetMin = new Vector2(_minPositionInPixels, _leftBracket.offsetMin.y);
            _leftBracket.offsetMax = new Vector2(_minPositionInPixels - _rectWidth, _leftBracket.offsetMax.y);

            _rightBracket.offsetMin = new Vector2(_maxPositionInPixels, _leftBracket.offsetMin.y);
            _rightBracket.offsetMax = new Vector2(_maxPositionInPixels - _rectWidth, _leftBracket.offsetMax.y);
        }
    }
}