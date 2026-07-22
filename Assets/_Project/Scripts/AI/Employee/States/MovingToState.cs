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
        private readonly EmployeeSoundEmitter soundEmitter;

        public MovingToState(NavMeshAgent agent, EmployeeConfig config, EmployeeBlackboard blackboard,
            EmployeeSoundEmitter soundEmitter)
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
            agent.SetDestination(blackboard.Destination);
            soundEmitter?.ResetTimer();
        }

        public override void OnLogic()
        {
            soundEmitter?.Tick(EmployeeSoundType.Walk, Time.deltaTime);
        }
    }
}
