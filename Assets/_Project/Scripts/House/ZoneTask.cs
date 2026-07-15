using UnityEngine;
using Game.AI.Employee;

namespace Game.House
{
    public sealed class ZoneTask : IEmployeeTask
    {
        private readonly Zone zone;
        private readonly ActivityDefinition activity;

        public Vector3 TargetPosition { get; }
        public float Duration => activity.Duration;
        public ActivityType ActivityType => activity.Type;

        public ZoneTask(Zone zone, Vector3 slotPosition, ActivityDefinition activity)
        {
            this.zone = zone;
            this.activity = activity;
            TargetPosition = slotPosition;
        }

        public void OnStarted() { }

        public void OnCompleted()
        {
            activity.Effect?.Apply(zone);
            zone.MarkActivityFinished(this);
        }

        public void OnCancelled() => zone.ReleaseSlot(this);
    }
}
