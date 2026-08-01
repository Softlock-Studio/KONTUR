using UnityEngine;
using UnityEngine.AI;

namespace Game.AI.Babooshka
{
    // Locomotion pattern mirrors EmployeeAnimatorDriver (same parameter names, so a shared
    // Invector-style controller could work for both), but pace is derived differently: the
    // employee accelerates continuously (AccelerationTime/BrakingDistance), while Babooshka's
    // speed is a discrete per-state constant (PatrolSpeed in Wander, ChaseSpeed in Chase/Search)
    // set directly on the agent — so pace is just current speed normalized against her fastest
    // speed. No IsSprinting equivalent either: she has no chase flag on the blackboard, and
    // adding one solely to drive a single bool parameter isn't worth it yet.
    //
    // No Animator/rig exists for Babooshka yet (as of writing) — this component is entirely
    // null-safe and simply does nothing until one is attached, same as EmployeeRagdoll before
    // its ragdoll rig existed.
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class BabooshkaAnimatorDriver : MonoBehaviour
    {
        private static readonly int InputHorizontal = Animator.StringToHash("InputHorizontal");
        private static readonly int InputVertical = Animator.StringToHash("InputVertical");
        private static readonly int InputMagnitude = Animator.StringToHash("InputMagnitude");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int IsStrafing = Animator.StringToHash("IsStrafing");
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int WallLickTrigger = Animator.StringToHash("WallLick");
        private static readonly int LightOffTrigger = Animator.StringToHash("LightOff");

        private const float MovingThreshold = 0.05f;

        [SerializeField] private float animationSmoothTime = 0.2f;

        private Animator animator;
        private NavMeshAgent agent;
        private BabooshkaConfig config;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
        }

        public void Bind(BabooshkaConfig babooshkaConfig)
        {
            config = babooshkaConfig;
        }

        private void Update()
        {
            if (animator == null || !animator.enabled) return;

            bool isMoving = agent.velocity.magnitude > MovingThreshold;
            float topSpeed = config != null && config.ChaseSpeed > 0f ? config.ChaseSpeed : 1f;
            float pace = isMoving ? Mathf.Clamp01(agent.velocity.magnitude / topSpeed) : 0f;

            Vector3 localDirection = isMoving
                ? transform.InverseTransformDirection(agent.velocity.normalized)
                : Vector3.zero;

            animator.SetFloat(InputVertical, localDirection.z * pace, animationSmoothTime, Time.deltaTime);
            animator.SetFloat(InputHorizontal, localDirection.x * pace, animationSmoothTime, Time.deltaTime);
            animator.SetFloat(InputMagnitude, pace, animationSmoothTime, Time.deltaTime);
            animator.SetBool(IsGrounded, true);
            animator.SetBool(IsStrafing, true);
        }

        public void PlayAttack() => animator?.SetTrigger(AttackTrigger);
        public void PlayWallLick() => animator?.SetTrigger(WallLickTrigger);
        public void PlayLightOff() => animator?.SetTrigger(LightOffTrigger);
    }
}
