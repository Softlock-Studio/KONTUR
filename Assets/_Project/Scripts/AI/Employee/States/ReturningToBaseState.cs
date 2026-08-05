using Game.AI;
using Game.Audio;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Employee
{
    public sealed class ReturningToBaseState : StateBase
    {
        private readonly NavMeshAgent agent;
        private readonly EmployeeConfig config;
        private readonly EmployeeBlackboard blackboard;
        private readonly LoopingSoundEmitter<EmployeeSoundType> soundEmitter;

        public ReturningToBaseState(NavMeshAgent agent, EmployeeConfig config, EmployeeBlackboard blackboard,
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
            agent.speed = config.ReturnSpeed;
            agent.isStopped = false;
            blackboard.DestinationUnreachable = false;
            if (blackboard.BasePoint != null) agent.SetDestination(blackboard.BasePoint.position);
            soundEmitter?.ResetTimer(EmployeeSoundType.Walk);
            // Breathing is deliberately not reset here — see MovingToState.OnEnter.
        }

        public override void OnLogic()
        {
            soundEmitter?.Tick(EmployeeSoundType.Walk, Time.deltaTime);
            soundEmitter?.Tick(EmployeeSoundType.Breathing, Time.deltaTime);

            if (blackboard.BasePoint != null && !blackboard.DestinationUnreachable && agent.HasUnreachableDestination())
                blackboard.DestinationUnreachable = true;
        }
    }
}
