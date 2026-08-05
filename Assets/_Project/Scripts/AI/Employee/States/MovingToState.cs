using Game.AI;
using Game.Audio;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Employee
{
    public sealed class MovingToState : StateBase
    {
        private readonly NavMeshAgent agent;
        private readonly EmployeeConfig config;
        private readonly EmployeeBlackboard blackboard;
        private readonly LoopingSoundEmitter<EmployeeSoundType> soundEmitter;

        public MovingToState(NavMeshAgent agent, EmployeeConfig config, EmployeeBlackboard blackboard,
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
            agent.speed = config.MoveSpeed;
            agent.isStopped = false;
            blackboard.DestinationUnreachable = false;
            agent.SetDestination(blackboard.Destination);
            soundEmitter?.ResetTimer(EmployeeSoundType.Walk);
            // Breathing is deliberately not reset here — like Babooshka's Laugh, it runs on its
            // own random cadence (EmployeeConfig.Sounds) independent of state entry, instead of
            // pulsing immediately every time the employee is sent somewhere new.
        }

        public override void OnLogic()
        {
            soundEmitter?.Tick(EmployeeSoundType.Walk, Time.deltaTime);
            soundEmitter?.Tick(EmployeeSoundType.Breathing, Time.deltaTime);

            if (!blackboard.DestinationUnreachable && agent.HasUnreachableDestination())
                blackboard.DestinationUnreachable = true;
        }
    }
}
