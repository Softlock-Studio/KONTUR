using UnityEngine;
using Game.AI.Employee;

namespace Game.House
{
    public sealed class ZoneTask : IEmployeeTask
    {
        private readonly Zone zone;

        public Vector3 TargetPosition { get; }
        public float Duration { get; }

        public ZoneTask(Zone zone, Vector3 slotPosition, float duration)
        {
            this.zone = zone;
            TargetPosition = slotPosition;
            Duration = duration;
        }

        public void OnStarted() { }

        public void OnCompleted() { }

        public void OnCancelled() => zone.ReleaseSlot(this);
    }
}
