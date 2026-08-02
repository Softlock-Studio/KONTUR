using Game.Audio;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Babooshka
{
    public sealed class ChaseState : StateBase
    {
        private readonly NavMeshAgent agent;
        private readonly BabooshkaConfig config;
        private readonly BabooshkaBlackboard blackboard;
        private readonly LoopingSoundEmitter<BabooshkaSoundType> soundEmitter;

        public ChaseState(NavMeshAgent agent, BabooshkaConfig config, BabooshkaBlackboard blackboard,
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

            // "Spotted!" bark — once per hunt, not on every Search→Chase re-acquisition of the
            // same employee. blackboard.LastAngeredTarget is forgotten in WanderState.OnEnter,
            // so a genuinely new encounter (this employee later, or a different one) barks again.
            if (blackboard.Target != null && blackboard.Target != blackboard.LastAngeredTarget)
            {
                soundEmitter?.ResetTimer(BabooshkaSoundType.Anger);
                blackboard.LastAngeredTarget = blackboard.Target;
            }
        }

        public override void OnLogic()
        {
            soundEmitter?.Tick(BabooshkaSoundType.Footstep, Time.deltaTime);
            soundEmitter?.Tick(BabooshkaSoundType.Anger, Time.deltaTime);
            soundEmitter?.Tick(BabooshkaSoundType.Taunt, Time.deltaTime);

            if (blackboard.Target == null) return;
            agent.SetDestination(blackboard.Target.Position);
        }
    }
}
