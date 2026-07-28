using Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingsButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button _button;

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

    private void Start()
    {
        _button.onClick.AddListener(() => CursorManager.Instance.ChangeCursor(CursorState.Default));
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }
}
