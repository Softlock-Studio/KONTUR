using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Babooshka
{
    public sealed class SearchState : StateBase
    {
        private readonly NavMeshAgent agent;
        private readonly BabooshkaConfig config;
        private readonly BabooshkaBlackboard blackboard;

        public SearchState(NavMeshAgent agent, BabooshkaConfig config, BabooshkaBlackboard blackboard)
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

            Vector3 destination = blackboard.LastSeenTime >= blackboard.LastHeardTime
                ? blackboard.LastKnownTargetPosition
                : blackboard.LastHeardSound;

            agent.SetDestination(destination);
        }
    }
}
