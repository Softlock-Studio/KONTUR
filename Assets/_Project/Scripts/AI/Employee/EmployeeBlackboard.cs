using Game.House;
using UnityEngine;

namespace Game.AI.Employee
{
    public sealed class EmployeeBlackboard
    {
        public Vector3 Destination;
        public IEmployeeTask PendingTask;
        public Transform BasePoint;

        // Zone the employee is currently moving to or working in — drives IEmployee.DestinationName.
        public Zone TargetZone;

        public bool FleeRequested;
        public bool IsFleeing;

        public bool AttackedRequested;
        public bool IsAttacked;

        // Set by Stop() when it interrupts an in-flight Move/AssignTask (not idle/returning-to-
        // base) — Continue() resumes exactly that command; any fresh command clears it again via
        // CancelCurrentTask.
        public bool Paused;
    }
}
