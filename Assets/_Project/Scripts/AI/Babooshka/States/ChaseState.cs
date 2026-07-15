using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Babooshka
{
    public sealed class ChaseState : StateBase
    {
        private readonly NavMeshAgent agent;
        private readonly BabooshkaConfig config;
        private readonly BabooshkaBlackboard blackboard;

        public ChaseState(NavMeshAgent agent, BabooshkaConfig config, BabooshkaBlackboard blackboard)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.config = config;
            this.blackboard = blackboard;
        }

        public override void OnEnter()
        {
            agent.speed = config.ChaseSpeed;
            agent.isStopped = false;
        }

        public override void OnLogic()
        {
            if (blackboard.Target == null) return;
            agent.SetDestination(blackboard.Target.Position);
        }
    }
}
