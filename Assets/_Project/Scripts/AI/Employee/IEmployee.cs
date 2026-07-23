using Game.House;
using UnityEngine;

namespace Game.AI.Employee
{
    public interface IEmployee
    {
        Vector3 Position { get; }
        bool IsAlive { get; }
        string CurrentStateName { get; }

        // Typed version of CurrentStateName for UI that needs to render/localize it (raw FSM
        // hierarchy paths like "/MovingTo" aren't fit for display) — backs Employee Card's "Goal".
        EmployeeStateId StateId { get; }

        // Display name of the zone this employee is currently moving to or working in; empty
        // when idle/stopped/returning to base/fleeing. Backs Employee Card's "Destination" field.
        string DestinationName { get; }

        bool AssignTask(IEmployeeTask task);
        void Move(Vector3 point, Zone targetZone = null);
        void Stop();
        void ReturnToBase();

        void ApplyAttackOutcome(bool survived);
    }
}
