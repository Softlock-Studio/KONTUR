using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Employee
{
    public sealed class MovingToState : StateBase
    {
        private readonly NavMeshAgent agent;
        private readonly EmployeeConfig config;
        private readonly EmployeeBlackboard blackboard;

        public MovingToState(NavMeshAgent agent, EmployeeConfig config, EmployeeBlackboard blackboard)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.config = config;
            this.blackboard = blackboard;
        }

        public override void OnEnter()
        {
            agent.speed = config.MoveSpeed;
            agent.isStopped = false;
            agent.SetDestination(blackboard.Destination);
        }
    }
}
