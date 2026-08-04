using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Employee
{
    /// <summary>Forced state entered when Babooshka attacks. Stands frozen until
    /// EmployeeController.ApplyAttackOutcome resolves it (ragdoll death disables the whole
    /// component immediately — a mid-animation cut to ragdoll reads fine). Surviving sets
    /// FleeRequested, but actually leaving for Fleeing additionally waits for HoldElapsed here —
    /// otherwise fleeing would start the instant Babooshka's own AttackReactionDuration expires,
    /// which is unrelated to (and can be shorter than) however long the employee's own
    /// hit-reaction animation/sequence actually plays, popping her straight from a static react
    /// pose into a run. Not player-cancellable — same shape as FleeingState.</summary>
    public sealed class AttackedState : StateBase
    {
        private readonly NavMeshAgent agent;
        private readonly EmployeeConfig config;
        private readonly EmployeeBlackboard blackboard;

        private float elapsed;

        public bool HoldElapsed { get; private set; }

        public AttackedState(NavMeshAgent agent, EmployeeConfig config, EmployeeBlackboard blackboard)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.config = config;
            this.blackboard = blackboard;
        }

        public override void OnEnter()
        {
            blackboard.AttackedRequested = false;
            blackboard.IsAttacked = true;
            agent.isStopped = true;
            elapsed = 0f;
            HoldElapsed = false;
        }

        public override void OnLogic()
        {
            if (HoldElapsed) return;

            elapsed += Time.deltaTime;
            if (elapsed >= config.AttackedHoldDurationSeconds) HoldElapsed = true;
        }

        public override void OnExit()
        {
            blackboard.IsAttacked = false;
        }
    }
}
