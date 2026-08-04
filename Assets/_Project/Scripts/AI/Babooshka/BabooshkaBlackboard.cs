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

        // Who she's already barked the "spotted!" Anger cue at this hunt — set on first Chase
        // entry, forgotten (null) once she fully disengages back to Wander, so re-spotting the
        // same employee later (or a different one) counts as a fresh encounter again.
        public Employee.IEmployee LastAngeredTarget;
    }
}
