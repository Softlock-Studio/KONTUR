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
        public int TreatmentIodineCost = 1;

        [Header("Lightbulb change activity (TBD placeholder values, not GDD-sourced)")]
        public float LightbulbChangeDurationSeconds = 5f;
        public int LightbulbChangeCost = 1;

        [Header("Resident event activity (TBD placeholder values, not GDD-sourced)")]
        public float ResidentEventDurationSeconds = 5f;

        [Header("Events (TBD placeholder values, not GDD-sourced)")]
        public List<ZoneEventDefinition> Events = new();

        [Header("Multi-worker speedup (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("X = number of employees currently actively working the same activity together, " +
                 "Y = total speed multiplier applied to that shared activity's progress.")]
        public AnimationCurve WorkerCountSpeedMultiplier = AnimationCurve.Linear(1f, 1f, 4f, 4f);

        public float EvaluateSpeedMultiplier(int activeParticipantCount)
        {
            if (activeParticipantCount <= 0) return 0f;
            return Mathf.Max(0f, WorkerCountSpeedMultiplier.Evaluate(activeParticipantCount));
        }
    }
}
