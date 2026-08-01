using Game.Audio;
using Game.Bootstrap;
using Game.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

[RequireComponent(typeof(Button))]
public class ActionMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button _button;
    [SerializeField] private SfxCue _clickCue;

    private IAudioService _audioService;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _audioService = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IAudioService>();
        _button.onClick.AddListener(() => { _audioService.PlayUiSfx(_clickCue); });
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
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
}
