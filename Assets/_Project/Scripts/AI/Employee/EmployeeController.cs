using System;
using Game.Audio;
using Game.House;
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
        [SerializeField] private EmployeeRagdoll ragdoll;
        [SerializeField] private EmployeeAnimatorDriver animatorDriver;
        [SerializeField] private AudioEmitter audioEmitter;
        [SerializeField] private Babooshka.HearingSensor[] hearingSensorsToNotify;

        // Set by hand per prefab instance — see IEmployee.CallsignNumber.
        [SerializeField] private int callsignNumber = 1;

        private NavMeshAgent agent;
        private StateMachine fsm;
        private EmployeeBlackboard blackboard;
        private LoopingSoundEmitter<EmployeeSoundType> soundEmitter;

        public Vector3 Position => transform.position;
        public bool IsAlive { get; private set; } = true;
        public string CurrentStateName => fsm?.GetActiveHierarchyPath() ?? string.Empty;
        public int CallsignNumber => callsignNumber;

        // States are flat (no sub-state-machines), so the hierarchy path is just "/<StateName>" —
        // trimming the slash lines up exactly with the names used in BuildStateMachine.
        public EmployeeStateId StateId =>
            Enum.TryParse(CurrentStateName.TrimStart('/'), out EmployeeStateId id) ? id : EmployeeStateId.Idle;

        public string DestinationName
        {
            get
            {
                Zone zone = blackboard?.TargetZone;
                if (zone == null) return string.Empty;
                return string.IsNullOrEmpty(zone.DisplayName) ? zone.name : zone.DisplayName;
            }
        }

        private bool CanAcceptCommand => IsAlive && !blackboard.IsFleeing && !blackboard.IsAttacked;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            blackboard = new EmployeeBlackboard { BasePoint = basePoint };
            ragdoll?.Bind(config);
            if (animatorDriver != null) animatorDriver.Bind(config, blackboard);
            soundEmitter = new LoopingSoundEmitter<EmployeeSoundType>(transform, audioEmitter, config.Sounds, NotifyHearingSensors);

            BuildStateMachine();
        }

        private void BuildStateMachine()
        {
            fsm = new StateMachine();

            fsm.AddState("Idle", new IdleState(agent));
            fsm.AddState("MovingTo", new MovingToState(agent, config, blackboard, soundEmitter));
            fsm.AddState("PerformingTask", new PerformingTaskState(agent, blackboard, soundEmitter));
            fsm.AddState("ReturningToBase", new ReturningToBaseState(agent, config, blackboard, soundEmitter));
            fsm.AddState("Fleeing", new FleeingState(agent, config, blackboard, soundEmitter));
            var attackedState = new AttackedState(agent, config, blackboard);
            fsm.AddState("Attacked", attackedState);

            fsm.SetStartState("Idle");

            fsm.AddTransition("MovingTo", "PerformingTask", t => HasArrived() && blackboard.PendingTask != null);
            fsm.AddTransition("MovingTo", "Idle", t => HasArrived() && blackboard.PendingTask == null,
                afterTransition: t => blackboard.TargetZone = null);
            fsm.AddTransition("ReturningToBase", "Idle", t => HasArrived());
            fsm.AddTransition("Fleeing", "Idle", t => HasArrived());

            fsm.AddTransition("PerformingTask", "Idle",
                t => blackboard.PendingTask != null && blackboard.PendingTask.IsComplete,
                afterTransition: t => CompleteCurrentTask());

            // While currently Attacked, additionally wait for the reaction hold — everywhere else
            // (Idle/MovingTo/PerformingTask/ReturningToBase, or a debug-triggered flee that never
            // went through Attacked at all) this is unaffected and stays instant.
            fsm.AddTransitionFromAny("Fleeing",
                t => blackboard.FleeRequested && (!blackboard.IsAttacked || attackedState.HoldElapsed),
                forceInstantly: true);
            fsm.AddTransitionFromAny("Attacked", t => blackboard.AttackedRequested, forceInstantly: true);

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

        // Passed to LoopingSoundEmitter as onEmitted — every sound this employee makes (footsteps,
        // cleaning, ...) also pings Babooshka's hearing, same as before the emitter was unified.
        private void NotifyHearingSensors(Vector3 position, SoundLoudness loudness)
        {
            if (hearingSensorsToNotify == null) return;
            foreach (Babooshka.HearingSensor sensor in hearingSensorsToNotify)
                sensor.NotifySound(position, loudness);
        }

        private void CompleteCurrentTask()
        {
            blackboard.PendingTask?.OnCompleted();
            blackboard.TargetZone = null;
        }

        // Fully abandons whatever's assigned (releases the Zone slot/session if any) — used by any
        // *new* command, including one that replaces a Stop()-paused task. Distinct from Stop()
        // itself, which keeps the task alive for Continue().
        private void CancelCurrentTask()
        {
            blackboard.TargetZone = null;
            blackboard.Paused = false;

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
            blackboard.TargetZone = task.SoundZone;
            fsm.RequestStateChange("MovingTo", forceInstantly: true);
            return true;
        }

        public void Move(Vector3 point, Zone targetZone = null)
        {
            if (!CanAcceptCommand) return;

            CancelCurrentTask();
            blackboard.Destination = point;
            blackboard.TargetZone = targetZone;
            fsm.RequestStateChange("MovingTo", forceInstantly: true);
        }

        public void Stop()
        {
            if (!CanAcceptCommand) return;

            // Only an in-flight Move/AssignTask is resumable — stopping while idle/returning/etc.
            // has nothing to remember, same as before.
            bool wasExecutingCommand = StateId == EmployeeStateId.MovingTo || StateId == EmployeeStateId.PerformingTask;
            if (wasExecutingCommand)
            {
                blackboard.PendingTask?.OnPaused();
                blackboard.Paused = true;
            }
            else
            {
                blackboard.Paused = false;
            }

            agent.ResetPath();
            fsm.RequestStateChange("Idle", forceInstantly: true);
        }

        public void Continue()
        {
            if (!CanAcceptCommand || !blackboard.Paused) return;

            // The paused task finished or was invalidated on its own while we were stopped (e.g.
            // another employee completed the shared activity) — nothing left to resume.
            if (blackboard.PendingTask != null && blackboard.PendingTask.IsComplete)
            {
                blackboard.PendingTask = null;
                blackboard.TargetZone = null;
                blackboard.Paused = false;
                return;
            }

            blackboard.Paused = false;
            fsm.RequestStateChange("MovingTo", forceInstantly: true);
        }

        public void ReturnToBase()
        {
            if (!CanAcceptCommand) return;

            CancelCurrentTask();
            fsm.RequestStateChange("ReturningToBase", forceInstantly: true);
        }

        public void ReactToAttack()
        {
            if (!IsAlive) return;

            CancelCurrentTask();
            blackboard.AttackedRequested = true;
            animatorDriver?.PlayAttacked();
            audioEmitter?.Play(config.AttackedCue);
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
                ragdoll?.TriggerDeath();
                audioEmitter?.Play(config.DeathCue);
                return;
            }

            CancelCurrentTask();
            blackboard.FleeRequested = true;
            audioEmitter?.Play(config.FleeCue);
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
