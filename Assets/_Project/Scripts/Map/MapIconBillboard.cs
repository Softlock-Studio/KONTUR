using Game.UI.House;
using UnityEngine;

namespace Game.Map
{
    // Also hides itself when its own floor isn't the one currently shown on the map.
    // FloorToggleView switches floors by moving the (orthographic) Map Camera between two preset
    // heights rather than toggling floor geometry, so without this an icon (e.g. an employee who
    // walked down to floor 1) stays visible on every floor's view, not just its own.
    public sealed class MapIconBillboard : MonoBehaviour
    {
        [SerializeField] private Vector3 fixedWorldEulerAngles = new Vector3(90f, 0f, 0f);
        [SerializeField] private float sameFloorHeightTolerance = 30f;

        private Renderer[] renderers;

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        private void LateUpdate()
        {
            transform.rotation = Quaternion.Euler(fixedWorldEulerAngles);
            UpdateFloorVisibility();
        }

        private void UpdateFloorVisibility()
        {
            if (!FloorToggleView.HasActiveFloor) return;

            bool visible = Mathf.Abs(transform.position.y - FloorToggleView.CurrentMapCameraY) <= sameFloorHeightTolerance;
            foreach (Renderer r in renderers)
                if (r != null) r.enabled = visible;
        }
    }
}
