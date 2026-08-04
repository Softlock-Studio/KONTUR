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
        private readonly EmployeeAnimatorDriver animatorDriver;

        public PerformingTaskState(NavMeshAgent agent, EmployeeBlackboard blackboard,
            LoopingSoundEmitter<EmployeeSoundType> soundEmitter, EmployeeAnimatorDriver animatorDriver = null)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.blackboard = blackboard;
            this.soundEmitter = soundEmitter;
            this.animatorDriver = animatorDriver;
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
            if (soundType.HasValue)
            {
                soundEmitter?.ResetTimer(soundType.Value);
                PlayTaskAnimation(soundType.Value);
            }
        }

        public override void OnLogic()
        {
            IEmployeeTask task = blackboard.PendingTask;
            EmployeeSoundType? soundType = task?.AmbientSoundType;
            if (soundType.HasValue) soundEmitter?.Tick(soundType.Value, Time.deltaTime);
        }

        // Only the two task types with an authored reaction animation trigger something here —
        // this intentionally does not also play a sound (see EmployeeAnimatorDriver.PlayCleaning).
        private void PlayTaskAnimation(EmployeeSoundType soundType)
        {
            switch (soundType)
            {
                case EmployeeSoundType.CleaningRoom: animatorDriver?.PlayCleaning(); break;
                case EmployeeSoundType.LightbulbChange: animatorDriver?.PlayLightbulbChange(); break;
            }
        }
    }
}
