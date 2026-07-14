using UnityEngine;

namespace Game.House
{
    [CreateAssetMenu(menuName = "House/Zone Config", fileName = "ZoneConfig")]
    public sealed class ZoneConfig : ScriptableObject
    {
        [Header("Infection growth (TBD — placeholder values, not GDD-sourced)")]
        public float BaseGrowthPerSecond = 0.05f;
        public float DarknessGrowthPerSecond = 0.1f;
    }
}
