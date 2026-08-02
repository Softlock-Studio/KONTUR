using Game.Audio;
using Game.Bootstrap;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Game.UI.Settings
{
    [RequireComponent(typeof(Button))]
    public class SettingsButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SfxCue _clickCue;
        [SerializeField] private Button _button;

        private IAudioService _audioService;            

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_button.IsInteractable())
                CursorManager.Instance.ChangeCursor(CursorState.Hover);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_button.IsInteractable())
                CursorManager.Instance.ChangeCursor(CursorState.Default);
        }

        private void ButtonClick()
        {
            CursorManager.Instance.ChangeCursor(CursorState.Default);
            _audioService.PlayUiSfx(_clickCue);
        }

        private void Start()
        {
            _audioService = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IAudioService>();
            _button.onClick.AddListener(ButtonClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(ButtonClick);
        }
    }
}