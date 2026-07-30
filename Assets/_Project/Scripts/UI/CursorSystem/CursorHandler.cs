using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    public class CursorHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            CursorManager.Instance.ChangeCursor(CursorState.Hover);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CursorManager.Instance.ChangeCursor(CursorState.Default);
        }
    }
}