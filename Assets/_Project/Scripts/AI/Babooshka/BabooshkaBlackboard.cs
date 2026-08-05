using UnityEngine;

namespace Game.AI.Babooshka
{
    public sealed class BabooshkaBlackboard
    {
        public Employee.IEmployee Target;
        public bool IsSlowed;

        public Vector3 LastKnownTargetPosition;
        public float LastSeenTime = float.NegativeInfinity;

        public Vector3 LastHeardSound;
        public float LastHeardTime = float.NegativeInfinity;

        public Employee.IEmployee SparedTarget;
        public float SparedUntilTime = float.NegativeInfinity;

        // Set by SightSensor when a newly-spotted employee fails the AggressionChance01 roll —
        // keeps that same employee un-targeted while they stay continuously in sight, without
        // re-rolling every frame. Cleared once nobody is visible, so the next sighting rolls fresh.
        public Employee.IEmployee IgnoredSightTarget;

        // Who she's already barked the "spotted!" Anger cue at this hunt — set on first Chase
        // entry, forgotten (null) once she fully disengages back to Wander, so re-spotting the
        // same employee later (or a different one) counts as a fresh encounter again.
        public Employee.IEmployee LastAngeredTarget;
    }
}
