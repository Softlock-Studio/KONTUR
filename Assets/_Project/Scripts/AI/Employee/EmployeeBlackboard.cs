using UnityEngine;

namespace Game.AI.Employee
{
    public sealed class EmployeeBlackboard
    {
        public Vector3 Destination;
        public IEmployeeTask PendingTask;
        public Transform BasePoint;

        public bool FleeRequested;
        public bool IsFleeing;
    }
}
