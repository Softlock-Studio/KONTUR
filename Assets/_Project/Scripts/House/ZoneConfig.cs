using System.Collections.Generic;
using UnityEngine;

namespace Game.House
{
    [CreateAssetMenu(menuName = "House/Zone Config", fileName = "ZoneConfig")]
    public sealed class ZoneConfig : ScriptableObject
    {
        [Header("Infection growth (TBD placeholder values, not GDD-sourced)")]
        public float BaseGrowthPerSecond = 0.05f;
        public float DarknessGrowthPerSecond = 0.1f;

        [Header("Treatment activity (TBD placeholder values, not GDD-sourced)")]
        public float TreatmentDurationSeconds = 5f;
        public float TreatmentInfectionReduction = 20f;

        [Header("Lightbulb change activity (TBD placeholder values, not GDD-sourced)")]
        public float LightbulbChangeDurationSeconds = 5f;

        [Header("Resident event activity (TBD placeholder values, not GDD-sourced)")]
        public float ResidentEventDurationSeconds = 5f;

        [Header("Events (TBD placeholder values, not GDD-sourced)")]
        public List<ZoneEventDefinition> Events = new();
    }
}
