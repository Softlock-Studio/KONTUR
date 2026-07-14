using UnityEngine;
using Game.AI.Employee;

namespace Game.House
{
    public sealed class ZoneTask : IEmployeeTask
    {
        private readonly Zone zone;
        private readonly IEmployee employee;

        public Vector3 TargetPosition { get; }
        public float Duration { get; }

        public ZoneTask(Zone zone, IEmployee employee, Vector3 slotPosition, float duration)
        {
            this.zone = zone;
            this.employee = employee;
            TargetPosition = slotPosition;
            Duration = duration;
        }

        public void OnStarted() { }
        public void OnCompleted() => zone.ReleaseSlot(employee);
        public void OnCancelled() => zone.ReleaseSlot(employee);
    }
}
