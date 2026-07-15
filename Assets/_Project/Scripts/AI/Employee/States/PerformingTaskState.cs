using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Employee
{
    public sealed class PerformingTaskState : StateBase
    {
        private readonly NavMeshAgent agent;
        private readonly EmployeeBlackboard blackboard;

        public PerformingTaskState(NavMeshAgent agent, EmployeeBlackboard blackboard)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.blackboard = blackboard;
        }

        public override void OnEnter()
        {
            agent.isStopped = true;
            blackboard.PendingTask?.OnStarted();
        }
    }
}
