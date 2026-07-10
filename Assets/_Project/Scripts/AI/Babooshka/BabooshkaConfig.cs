using UnityEngine;

namespace Game.AI.Babooshka
{
    [CreateAssetMenu(menuName = "AI/Babooshka Config", fileName = "BabooshkaConfig")]
    public sealed class BabooshkaConfig : ScriptableObject
    {
        [Header("Movement")]
        public float PatrolSpeed = 1.5f;
        public float ChaseSpeed = 3f;

        [Header("Senses")]
        public float SightRadius = 10f;
        [Range(0f, 360f)] public float SightAngle = 110f;
        public float HearingRadius = 12f;
        public float HearingReactionWindow = 0.3f;
        public LayerMask EmployeeLayer;
        public LayerMask ObstacleMask;

        [Header("Fight")]
        public float AttackRange = 1.5f;
        public float FightResolutionDuration = 1.5f;
        public float InvestigateTimeout = 6f;

        [Header("Death chance")]
        public AnimationCurve DeathChanceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public float ResolveDeathChance(float infection)
        {
            return DeathChanceCurve.Evaluate(Mathf.Clamp01(infection));
        }
    }
}
