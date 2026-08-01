using Game.Audio;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Employee
{
    public sealed class PerformingTaskState : StateBase
    {
        private readonly NavMeshAgent agent;
        private readonly EmployeeBlackboard blackboard;
        private readonly LoopingSoundEmitter<EmployeeSoundType> soundEmitter;

        public PerformingTaskState(NavMeshAgent agent, EmployeeBlackboard blackboard,
            LoopingSoundEmitter<EmployeeSoundType> soundEmitter)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.blackboard = blackboard;
            this.soundEmitter = soundEmitter;
        }

        public override void OnEnter()
        {
            agent.isStopped = true;

            IEmployeeTask task = blackboard.PendingTask;
            if (task != null && !task.OnStarted())
            {
                task.OnCancelled();
                blackboard.PendingTask = null;
                return;
            }

            // Reset only after OnStarted() succeeds and the task's sound type is known — the
            // emitter can't reset a timer for a type it doesn't have yet, and there's nothing to
            // pulse immediately for a task that emits no ambient sound at all.
            EmployeeSoundType? soundType = task?.AmbientSoundType;
            if (soundType.HasValue) soundEmitter?.ResetTimer(soundType.Value);
        }

        public override void OnLogic()
        {
            IEmployeeTask task = blackboard.PendingTask;
            EmployeeSoundType? soundType = task?.AmbientSoundType;
            if (soundType.HasValue) soundEmitter?.Tick(soundType.Value, Time.deltaTime);
        }
    }
}
