using UnityEngine;

namespace Game.AI.Employee
{
    public sealed class DebugEmployeeTask : IEmployeeTask
    {
        private readonly float duration;
        private float startTime;

        public Vector3 TargetPosition { get; }
        public bool IsComplete => Time.time - startTime >= duration;
        public EmployeeSoundType? AmbientSoundType => null;

        public DebugEmployeeTask(Vector3 targetPosition, float duration)
        {
            TargetPosition = targetPosition;
            this.duration = duration;
        }

        public bool OnStarted()
        {
            startTime = Time.time;
            Debug.Log("[DebugEmployeeTask] Started");
            return true;
        }

        public void OnCompleted() => Debug.Log("[DebugEmployeeTask] Completed");
        public void OnCancelled() => Debug.Log("[DebugEmployeeTask] Cancelled");
    }
}
