namespace Game.House
{
    // Shared progress pool for one ongoing collaborative activity: every employee performing the
    // same activity (same zone, same ActivityType/target event) ticks this same session, so
    // joining or leaving mid-way changes the speed for everyone still on it, instead of each
    // employee running its own independent timer.
    internal sealed class ZoneActivitySession
    {
        public ActivityDefinition Activity { get; }
        public float Progress { get; private set; }
        public bool EffectApplied { get; private set; }

        // Employees currently actively performing (arrived, resource check passed) — drives speed.
        public int ActiveParticipantCount { get; private set; }

        // Employees currently assigned to this session at all (walking or working) — keeps the
        // session alive in the zone's registry so a still-arriving second employee can still join
        // it instead of starting a separate, unsynced session.
        public int ReferenceCount { get; private set; }

        public ZoneActivitySession(ActivityDefinition activity)
        {
            Activity = activity;
        }

        public void AddReference() => ReferenceCount++;
        public void RemoveReference() => ReferenceCount = ReferenceCount > 0 ? ReferenceCount - 1 : 0;

        public void Join() => ActiveParticipantCount++;
        public void Leave() => ActiveParticipantCount = ActiveParticipantCount > 0 ? ActiveParticipantCount - 1 : 0;

        // Returns true exactly once, on the tick progress crosses the completion threshold.
        public bool Advance(float amount)
        {
            if (EffectApplied) return false;

            Progress += amount;
            if (Progress < Activity.Duration) return false;

            EffectApplied = true;
            return true;
        }
    }
}
