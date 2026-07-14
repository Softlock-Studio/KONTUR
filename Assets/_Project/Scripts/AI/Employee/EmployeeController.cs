using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.AI.Employee
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class EmployeeController : MonoBehaviour, IEmployee
    {
        [SerializeField] private EmployeeConfig config;
        [SerializeField] private Transform basePoint;

        private NavMeshAgent agent;
        private StateMachine fsm;
        private EmployeeBlackboard blackboard;

        public Vector3 Position => transform.position;
        public bool IsAlive { get; private set; } = true;
        public string CurrentStateName => fsm?.GetActiveHierarchyPath() ?? string.Empty;

        private bool CanAcceptCommand => IsAlive && !blackboard.IsFleeing;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            blackboard = new EmployeeBlackboard { BasePoint = basePoint };

            BuildStateMachine();
        }

        private void BuildStateMachine()
        {
            fsm = new StateMachine();

            fsm.AddState("Idle", new IdleState(agent));
            fsm.AddState("MovingTo", new MovingToState(agent, config, blackboard));
            fsm.AddState("PerformingTask", new PerformingTaskState(agent, blackboard));
            fsm.AddState("ReturningToBase", new ReturningToBaseState(agent, config, blackboard));
            fsm.AddState("Fleeing", new FleeingState(agent, config, blackboard));

            fsm.SetStartState("Idle");

            fsm.AddTransition("MovingTo", "PerformingTask", t => HasArrived() && blackboard.PendingTask != null);
            fsm.AddTransition("MovingTo", "Idle", t => HasArrived() && blackboard.PendingTask == null);
            fsm.AddTransition("ReturningToBase", "Idle", t => HasArrived());
            fsm.AddTransition("Fleeing", "Idle", t => HasArrived());

            fsm.AddTransition(new TransitionAfterDynamic(
                "PerformingTask", "Idle",
                delay: t => blackboard.PendingTask?.Duration ?? 0f,
                onlyEvaluateDelayOnEnter: true,
                afterTransition: t => CompleteCurrentTask()));

            fsm.AddTransitionFromAny("Fleeing", t => blackboard.FleeRequested, forceInstantly: true);

            fsm.StateChanged += state => Debug.Log($"[{name}] FSM: {state.name}", this);

            fsm.Init();
        }

        private void Update()
        {
            fsm.OnLogic();
        }

        private bool HasArrived()
        {
            return !agent.pathPending && agent.remainingDistance <= config.ArrivalThreshold;
        }

        private void CompleteCurrentTask()
        {
            blackboard.PendingTask?.OnCompleted();
            blackboard.PendingTask = null;
        }

        private void CancelCurrentTask()
        {
            if (blackboard.PendingTask == null) return;
            blackboard.PendingTask.OnCancelled();
            blackboard.PendingTask = null;
        }

        public bool AssignTask(IEmployeeTask task)
        {
            if (!CanAcceptCommand || task == null) return false;

            CancelCurrentTask();
            blackboard.PendingTask = task;
            blackboard.Destination = task.TargetPosition;
            fsm.RequestStateChange("MovingTo", forceInstantly: true);
            return true;
        }

        public void Move(Vector3 point)
        {
            if (!CanAcceptCommand) return;

            CancelCurrentTask();
            blackboard.Destination = point;
            fsm.RequestStateChange("MovingTo", forceInstantly: true);
        }

        public void Stop()
        {
            if (!CanAcceptCommand) return;

            CancelCurrentTask();
            agent.ResetPath();
            fsm.RequestStateChange("Idle", forceInstantly: true);
        }

        public void ReturnToBase()
        {
            if (!CanAcceptCommand) return;

            CancelCurrentTask();
            fsm.RequestStateChange("ReturningToBase", forceInstantly: true);
        }

        public void ApplyAttackOutcome(bool survived)
        {
            if (!IsAlive) return;

            if (!survived)
            {
                IsAlive = false;
                CancelCurrentTask();
                agent.isStopped = true;
                enabled = false;
                return;
            }

            CancelCurrentTask();
            blackboard.FleeRequested = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (fsm == null) return;
            Handles.Label(transform.position + Vector3.up * 2.2f, CurrentStateName);
        }
#endif
    }
}
