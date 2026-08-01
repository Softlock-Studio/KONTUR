using System.Collections.Generic;
using Game.Audio;
using UnityEngine;

namespace Game.AI.Babooshka
{
    [CreateAssetMenu(menuName = "AI/Babooshka Config", fileName = "BabooshkaConfig")]
    public sealed class BabooshkaConfig : ScriptableObject
    {
        [Header("Movement")]
        public float PatrolSpeed = 1.5f;
        public float ChaseSpeed = 3f;

        [Header("Wander")]
        public float WanderStandStillMinSeconds = 1.5f;
        public float WanderStandStillMaxSeconds = 5f;
        [Range(0f, 1f)] public float ApartmentVisitChance = 0.3f;
        [Tooltip("Rolled once per stand-still while inside an apartment zone.")]
        [Range(0f, 1f)] public float WallLickChance = 0.35f;
        [Tooltip("Rolled once per stand-still while inside an apartment zone, only if the wall-lick roll above didn't already succeed (at most one \"creepy event\" per visit) — actual odds are (1 - WallLickChance) * LightOffChance, not this value directly.")]
        [Range(0f, 1f)] public float LightOffChance = 0.2f;

        [Header("Senses")]
        public float SightRadius = 10f;
        [Range(0f, 360f)] public float SightAngle = 110f;
        public float HearingRadius = 12f;
        public float HearingReactionWindow = 0.3f;
        public LayerMask EmployeeLayer;
        public LayerMask ObstacleMask;

        [Header("Hearing — loudness scaling (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Multiplies HearingRadius depending on how loud the sound was.")]
        public float LowLoudnessRadiusMultiplier = 0.5f;
        public float MediumLoudnessRadiusMultiplier = 1f;
        public float HighLoudnessRadiusMultiplier = 1.75f;

        [Header("Hearing — cross-floor (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Vertical (Y) distance within which a sound counts as coming from the same floor as this sensor. No separate floor/level tracking exists yet, so this is the stand-in for \"same floor\".")]
        public float SameFloorHeightTolerance = 2.5f;
        [Tooltip("Multiplies the effective hearing radius for sounds coming from a different floor — keep below 1 to make them harder to notice than same-floor sounds.")]
        [Range(0f, 1f)] public float DifferentFloorRadiusMultiplier = 0.35f;

        public float GetHearingRadius(SoundLoudness loudness, bool sameFloor)
        {
            float multiplier = loudness switch
            {
                SoundLoudness.Low => LowLoudnessRadiusMultiplier,
                SoundLoudness.High => HighLoudnessRadiusMultiplier,
                _ => MediumLoudnessRadiusMultiplier,
            };

            float radius = HearingRadius * multiplier;
            if (!sameFloor) radius *= DifferentFloorRadiusMultiplier;
            return radius;
        }

        [Header("Fight")]
        public float AttackRange = 1.5f;
        [Tooltip("How long Babooshka and the targeted employee stay frozen in their attack/reaction animations before the survive/die outcome is applied.")]
        public float AttackReactionDuration = 1f;
        [Tooltip("How long Babooshka lingers after the outcome is applied (death/flee), before releasing back to Wander.")]
        public float FightResolutionDuration = 1.5f;
        public float InvestigateTimeout = 6f;
        [Tooltip("How long a survivor is invisible to SightSensor right after a fight, so they get a chance to flee instead of being instantly re-engaged.")]
        public float PostFightMercyDuration = 3f;

        [Header("Death chance")]
        public AnimationCurve DeathChanceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Audio")]
        public SfxCue WallLickCue;
        public SfxCue LightOffCue;
        public SfxCue AttackCue;

        [Header("Sounds (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Per-type re-trigger cadence — see SoundDefinition.MinIntervalSeconds/MaxIntervalSeconds. " +
                 "Footstep/Laugh/Anger — Attack has its own one-shot AttackCue above instead, it's not periodic.")]
        public List<SoundDefinition<BabooshkaSoundType>> Sounds = new();

        [Header("Debug")]
        [Tooltip("Gizmos (sight cone, hearing radius, state label) and console logs for this Babooshka.")]
        public bool EnableDebugVisuals = false;

        public float ResolveDeathChance(float infection)
        {
            return DeathChanceCurve.Evaluate(Mathf.Clamp01(infection));
        }
    }
}
