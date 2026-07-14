using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Employee
{
    public sealed class FleeingState : StateBase
    {
        private readonly NavMeshAgent agent;
        private readonly EmployeeConfig config;
        private readonly EmployeeBlackboard blackboard;

        public FleeingState(NavMeshAgent agent, EmployeeConfig config, EmployeeBlackboard blackboard)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.config = config;
            this.blackboard = blackboard;
        }

        public override void OnEnter()
        {
            blackboard.FleeRequested = false;
            blackboard.IsFleeing = true;
            agent.speed = config.FleeSpeed;
            agent.isStopped = false;
            if (blackboard.BasePoint != null) agent.SetDestination(blackboard.BasePoint.position);
        }

        public override void OnExit()
        {
            blackboard.IsFleeing = false;
        }
    }
}
