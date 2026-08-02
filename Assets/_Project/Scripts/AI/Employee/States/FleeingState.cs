using Game.Audio;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Employee
{
    /// <summary>Forced state entered when an employee survives a Babooshka encounter. Not player-cancellable.</summary>
    public sealed class FleeingState : StateBase
    {
        private readonly NavMeshAgent agent;
        private readonly EmployeeConfig config;
        private readonly EmployeeBlackboard blackboard;
        private readonly LoopingSoundEmitter<EmployeeSoundType> soundEmitter;

        public FleeingState(NavMeshAgent agent, EmployeeConfig config, EmployeeBlackboard blackboard,
            LoopingSoundEmitter<EmployeeSoundType> soundEmitter)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.config = config;
            this.blackboard = blackboard;
            this.soundEmitter = soundEmitter;
        }

        public override void OnEnter()
        {
            blackboard.FleeRequested = false;
            blackboard.IsFleeing = true;
            agent.speed = config.FleeSpeed;
            agent.isStopped = false;
            if (blackboard.BasePoint != null) agent.SetDestination(blackboard.BasePoint.position);
            soundEmitter?.ResetTimer(EmployeeSoundType.Run);
        }

        public override void OnLogic()
        {
            soundEmitter?.Tick(EmployeeSoundType.Run, Time.deltaTime);
        }

        public override void OnExit()
        {
            blackboard.IsFleeing = false;
        }
    }
}
