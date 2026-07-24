using UnityEngine;
using Game.AI.Employee;

namespace Game.House
{
    public sealed class ZoneTask : IEmployeeTask
    {
        private readonly Zone zone;
        private readonly ZoneActivitySession session;

        // "started" latches once (resource charged, first Join() done) and never resets — OnStarted
        // must not re-charge the resource on a Stop()->Continue() resume. "contributing" tracks
        // whether we're *currently* counted in the shared session's ActiveParticipantCount, which
        // does toggle off/on across a pause/resume (OnPaused -> Leave, OnStarted resume -> Join).
        private bool started;
        private bool contributing;

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
            if (started)
            {
                // Resuming after OnPaused() — resource already charged the first time, just rejoin
                // the active headcount.
                if (!contributing)
                {
                    session.Join();
                    contributing = true;
                }

                return true;
            }

            ActivityDefinition activity = session.Activity;
            if (activity.ResourceType.HasValue
                && !zone.TrySpendResource(activity.Type, activity.ResourceType.Value, activity.ResourceCost))
                return false;

            started = true;
            contributing = true;
            session.Join();
            return true;
        }

        public void OnCompleted() => zone.MarkActivityFinished(this);

        public void OnPaused()
        {
            if (!contributing) return;
            session.Leave();
            contributing = false;
        }

        public void OnCancelled()
        {
            if (contributing) session.Leave();
            session.RemoveReference();
            zone.ReleaseSlot(this);
        }
    }
}
