using UnityEngine;

namespace Game.AI.Employee
{
    public sealed class DebugEmployeeTask : IEmployeeTask
    {
        public Vector3 TargetPosition { get; }
        public float Duration { get; }

        public DebugEmployeeTask(Vector3 targetPosition, float duration)
        {
            TargetPosition = targetPosition;
            Duration = duration;
        }

        public void OnStarted() => Debug.Log("[DebugEmployeeTask] Started");
        public void OnCompleted() => Debug.Log("[DebugEmployeeTask] Completed");
        public void OnCancelled() => Debug.Log("[DebugEmployeeTask] Cancelled");
    }
}
