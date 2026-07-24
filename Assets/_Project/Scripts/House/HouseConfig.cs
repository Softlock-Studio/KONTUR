using UnityEngine;

namespace Game.House
{
    [CreateAssetMenu(menuName = "House/House Config", fileName = "HouseConfig")]
    public class HouseConfig : ScriptableObject
    {
        [Header("Mission Timer")]
        public double DayDurationInSecond;

        // No cross-night progression system exists yet (see Docs/agents/gdd/nights.md) — this is
        // just "which night is this level/scene", set per HouseConfig instance by hand.
        [Header("Night")]
        public int NightNumber = 1;
    }
}