using UnityEngine;
using Game.AI.Employee;

namespace Game.House
{
    public sealed class ZoneTask : IEmployeeTask
    {
        private readonly Zone zone;
        private readonly ZoneActivitySession session;
        private bool joined;

        public Vector3 TargetPosition { get; }
        public ActivityType ActivityType => session.Activity.Type;
        public bool IsComplete => session.EffectApplied;

        public EmployeeSoundType? AmbientSoundType => ActivityType switch
        {
            ActivityType.Treatment => EmployeeSoundType.CleaningRoom,
            ActivityType.LightbulbChange => EmployeeSoundType.LightbulbChange,
            _ => null,
        };

        public Zone SoundZone => zone;

        internal ZoneTask(Zone zone, Vector3 slotPosition, ZoneActivitySession session)
        {
            this.zone = zone;
            this.session = session;
            TargetPosition = slotPosition;
        }

        public bool OnStarted()
        {
            ActivityDefinition activity = session.Activity;
            if (activity.ResourceType.HasValue
                && !zone.TrySpendResource(activity.Type, activity.ResourceType.Value, activity.ResourceCost))
                return false;

            joined = true;
            session.Join();
            return true;
        }

        public void OnCompleted() => zone.MarkActivityFinished(this);

        public void OnCancelled()
        {
            if (joined) session.Leave();
            session.RemoveReference();
            zone.ReleaseSlot(this);
        }
    }
}
