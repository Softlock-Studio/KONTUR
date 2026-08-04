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
        [Tooltip("Высота (Y) камеры карты для первого этажа — временное решение переключения этажей, см. FloorToggleView.")]
        public float Floor1ViewY = -7.3f;
        [Tooltip("Высота (Y) камеры карты для второго этажа — временное решение переключения этажей, см. FloorToggleView.")]
        public float Floor2ViewY = 180f;

        [Header("Map click raycast")]
        [Tooltip("Слои, по которым проходит рейкаст при клике по карте.")]
        public LayerMask RaycastMask = ~0;
        [Tooltip("Максимальная дистанция рейкаста при клике по карте.")]
        public float MaxRayDistance = 1000f;
    }
}
