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
    }
}
