using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Employee
{
    public sealed class ReturningToBaseState : StateBase
    {
        private readonly NavMeshAgent agent;
        private readonly EmployeeConfig config;
        private readonly EmployeeBlackboard blackboard;

        public ReturningToBaseState(NavMeshAgent agent, EmployeeConfig config, EmployeeBlackboard blackboard)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.config = config;
            this.blackboard = blackboard;
        }

        public override void OnEnter()
        {
            agent.speed = config.ReturnSpeed;
            agent.isStopped = false;
            if (blackboard.BasePoint != null) agent.SetDestination(blackboard.BasePoint.position);
        }
    }
}
