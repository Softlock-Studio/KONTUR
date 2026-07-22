using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Employee
{
    public sealed class PerformingTaskState : StateBase
    {
        private readonly NavMeshAgent agent;
        private readonly EmployeeBlackboard blackboard;
        private readonly EmployeeSoundEmitter soundEmitter;

        public PerformingTaskState(NavMeshAgent agent, EmployeeBlackboard blackboard, EmployeeSoundEmitter soundEmitter)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.blackboard = blackboard;
            this.soundEmitter = soundEmitter;
        }

        public override void OnEnter()
        {
            agent.isStopped = true;
            soundEmitter?.ResetTimer();

            IEmployeeTask task = blackboard.PendingTask;
            if (task != null && !task.OnStarted())
            {
                task.OnCancelled();
                blackboard.PendingTask = null;
            }
        }

        public override void OnLogic()
        {
            IEmployeeTask task = blackboard.PendingTask;
            EmployeeSoundType? soundType = task?.AmbientSoundType;
            if (soundType.HasValue) soundEmitter?.Tick(soundType.Value, Time.deltaTime, task.SoundZone);
        }
    }
}
