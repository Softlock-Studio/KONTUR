using UnityEngine;
using UnityEngine.AI;

namespace Game.AI.Employee
{
    // Drives the Invector "BasicLocomotion" Animator Controller purely from NavMeshAgent
    // velocity. Movement and rotation stay fully owned by the NavMeshAgent — this component
    // never touches the transform, it only feeds the blend tree parameters Invector's own
    // vThirdPersonAnimator would otherwise compute from player input.
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class EmployeeAnimatorDriver : MonoBehaviour
    {
        private static readonly int InputHorizontal = Animator.StringToHash("InputHorizontal");
        private static readonly int InputVertical = Animator.StringToHash("InputVertical");
        private static readonly int InputMagnitude = Animator.StringToHash("InputMagnitude");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int IsStrafing = Animator.StringToHash("IsStrafing");
        private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");

        [SerializeField] private float animationSmoothTime = 0.2f;

        private Animator animator;
        private NavMeshAgent agent;
        private EmployeeConfig config;
        private EmployeeBlackboard blackboard;
        private float moveElapsedTime;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
        }

        public void Bind(EmployeeConfig employeeConfig, EmployeeBlackboard employeeBlackboard)
        {
            config = employeeConfig;
            blackboard = employeeBlackboard;
        }

        // Invector calibrates its locomotion blend tree against fixed magnitudes
        // (0.5 = walk, 1 = run), not raw agent speed, so pace is picked from state
        // rather than derived from agent.velocity / agent.speed — that ratio snaps
        // close to 1 almost immediately once the agent accelerates, which reads as
        // an instant sprint instead of a walk.
        private const float WalkPace = 0.5f;
        private const float RunPace = 1f;
        private const float MovingThreshold = 0.05f;

        private void Update()
        {
            if (animator == null || !animator.enabled) return;

            bool isFleeing = blackboard != null && blackboard.IsFleeing;
            bool isMoving = agent.velocity.magnitude > MovingThreshold;
            moveElapsedTime = isMoving ? moveElapsedTime + Time.deltaTime : 0f;

            // Pace ramps from a walk to a run over AccelerationTime seconds of continuous
            // movement, but is also capped by how much path distance is left (BrakingDistance)
            // — so a short errand never has time (or room) to accelerate and stays a walk,
            // while a long trip speeds up and decelerates back to a walk before arriving.
            float pace = 0f;
            if (isMoving)
            {
                if (isFleeing)
                {
                    pace = RunPace;
                }
                else
                {
                    float accelerationTime = config != null ? config.AccelerationTime : 3f;
                    float brakingDistance = config != null ? config.BrakingDistance : 4f;
                    float remainingDistance = agent.pathPending ? float.MaxValue : agent.remainingDistance;

                    float timeFactor = accelerationTime > 0f ? Mathf.Clamp01(moveElapsedTime / accelerationTime) : 1f;
                    float distanceFactor = brakingDistance > 0f ? Mathf.Clamp01(remainingDistance / brakingDistance) : 1f;

                    pace = Mathf.Lerp(WalkPace, RunPace, Mathf.Min(timeFactor, distanceFactor));
                }
            }

            // IsStrafing routes into the "Strafe Locomotion" -> "Strafing Movement" blend
            // tree, which has real sideways/diagonal footwork — unlike FreeLocomotion
            // (Idle/Walk/Run/Sprint), which only ever plays a forward gait and made turns
            // look like sliding once a horizontal component got blended into it.
            Vector3 localDirection = isMoving
                ? transform.InverseTransformDirection(agent.velocity.normalized)
                : Vector3.zero;

            animator.SetFloat(InputVertical, localDirection.z * pace, animationSmoothTime, Time.deltaTime);
            animator.SetFloat(InputHorizontal, localDirection.x * pace, animationSmoothTime, Time.deltaTime);
            animator.SetFloat(InputMagnitude, pace, animationSmoothTime, Time.deltaTime);
            animator.SetBool(IsGrounded, true);
            animator.SetBool(IsStrafing, true);
            animator.SetBool(IsSprinting, isFleeing);
        }
    }
}
