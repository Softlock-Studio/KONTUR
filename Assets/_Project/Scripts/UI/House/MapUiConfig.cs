using UnityEngine;

namespace Game.UI.House
{
    // Per-scene tunables for MapClickController/FloorToggleView — floor heights differ per house
    // layout, so create one instance per scene (same pattern as ZoneApartmentConfig/
    // ZoneCorridorConfig/ZoneStairsConfig: one class, several instances for different contexts).
    [CreateAssetMenu(menuName = "KONTUR/UI/Map Ui Config", fileName = "MapUiConfig")]
    public sealed class MapUiConfig : ScriptableObject
    {
        [Header("Floor toggle — temporary Map Camera height hack, see FloorToggleView")]
        public float Floor1ViewY = -7.3f;
        public float Floor2ViewY = 180f;

        [Header("Map click raycast")]
        public LayerMask RaycastMask = ~0;
        public float MaxRayDistance = 1000f;
    }
}
