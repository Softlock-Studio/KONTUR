using Game.Audio;
using Game.Bootstrap;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Game.UI.Employees
{
    public class EmployeeCardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _hoverSprite;
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private SfxCue _clickCue;

        private IAudioService _audioService;

        private void Start()
        {
            _audioService = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IAudioService>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _audioService.PlayUiSfx(_clickCue);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            CursorManager.Instance.ChangeCursor(CursorState.Hover);

            if (_backgroundImage.sprite == _selectedSprite) return;

            _backgroundImage.sprite = _hoverSprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CursorManager.Instance.ChangeCursor(CursorState.Default);

            if (_backgroundImage.sprite == _selectedSprite) return;

            _backgroundImage.sprite = _defaultSprite;
        }

        public void SetSelectedSprite(bool isSelected)
        {
            _backgroundImage.sprite = isSelected ? _selectedSprite : _defaultSprite;
        }
    }
}