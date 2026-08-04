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

        // Target infection range for this level. Checked once, at night end (see
        // MissionManager.Tick) — infection outside [Floor; Ceiling] when the timer runs out is a
        // defeat, same as the existing "hit 100%" hard cap but evaluated only at day-end rather
        // than instantly. No cross-night escalation yet — tune per level by hand, same as NightNumber.
        [Header("Infection Corridor")]
        [Range(0f, 1f)] public float InfectionFloor01 = 0.2f;
        [Range(0f, 1f)] public float InfectionCeiling01 = 0.4f;
    }
}