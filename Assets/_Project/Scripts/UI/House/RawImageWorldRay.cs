using UnityEngine;

namespace Game.UI.House
{
    // Converts a UI click on a RawImage (fed by some other Camera's RenderTexture, e.g. the
    // top-down Map Camera) into a world-space ray from that source camera — shared by any
    // map-click interaction (zone assignment, camera-icon selection).
    public static class RawImageWorldRay
    {
        public static bool TryGetWorldRay(RectTransform rawImageRect, Camera sourceCamera,
            Vector2 screenPoint, Camera eventCamera, out Ray ray)
        {
            ray = default;
            if (rawImageRect == null || sourceCamera == null) return false;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImageRect, screenPoint, eventCamera, out Vector2 local))
                return false;

            Rect r = rawImageRect.rect;
            float u = Mathf.InverseLerp(r.xMin, r.xMax, local.x);
            float v = Mathf.InverseLerp(r.yMin, r.yMax, local.y);

            ray = sourceCamera.ViewportPointToRay(new Vector3(u, v, 0f));
            return true;
        }
    }
}
