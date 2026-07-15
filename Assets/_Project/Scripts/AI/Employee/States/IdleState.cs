using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Employee
{
    public sealed class IdleState : StateBase
    {
        private readonly NavMeshAgent agent;

        public IdleState(NavMeshAgent agent) : base(needsExitTime: false)
        {
            this.agent = agent;
        }

        public override void OnEnter()
        {
            agent.isStopped = true;
        }
    }
}
