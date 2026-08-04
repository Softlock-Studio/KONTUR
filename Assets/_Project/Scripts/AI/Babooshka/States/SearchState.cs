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

            Vector3 destination = blackboard.LastSeenTime >= blackboard.LastHeardTime
                ? blackboard.LastKnownTargetPosition
                : blackboard.LastHeardSound;

            agent.SetDestination(destination);
        }

        public override void OnLogic()
        {
            soundEmitter?.Tick(BabooshkaSoundType.Footstep, Time.deltaTime);
        }
    }
}
