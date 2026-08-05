using Game.AI;
using Game.Audio;
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
        private readonly LoopingSoundEmitter<BabooshkaSoundType> soundEmitter;

        // Checked by BabooshkaController to bail out to Wander early instead of waiting out the
        // full InvestigateTimeout when the search destination can't be reached at all.
        public bool DestinationUnreachable { get; private set; }

        public SearchState(NavMeshAgent agent, BabooshkaConfig config, BabooshkaBlackboard blackboard,
            LoopingSoundEmitter<BabooshkaSoundType> soundEmitter = null)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.config = config;
            this.blackboard = blackboard;
            this.soundEmitter = soundEmitter;
        }

        public override void OnEnter()
        {
            agent.speed = config.ChaseSpeed;
            agent.isStopped = false;
            DestinationUnreachable = false;

            Vector3 destination = blackboard.LastSeenTime >= blackboard.LastHeardTime
                ? blackboard.LastKnownTargetPosition
                : blackboard.LastHeardSound;

            agent.SetDestination(destination);
        }

        public override void OnLogic()
        {
            soundEmitter?.Tick(BabooshkaSoundType.Footstep, Time.deltaTime);

            if (!DestinationUnreachable && agent.HasUnreachableDestination())
                DestinationUnreachable = true;
        }
    }
}
