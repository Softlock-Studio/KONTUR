using Game.House;
using UnityEngine;

namespace Game.AI.Employee
{
    public interface IEmployee
    {
        Vector3 Position { get; }
        bool IsAlive { get; }
        string CurrentStateName { get; }

        // Fixed per-instance roster number ("MOS #3") — designer-authored on the prefab, not
        // computed at runtime, so it's stable for the employee's whole lifetime and never reused
        // by anyone else even after this employee dies. Backs Employee Card's "Name" field.
        int CallsignNumber { get; }

        // Typed version of CurrentStateName for UI that needs to render/localize it (raw FSM
        // hierarchy paths like "/MovingTo" aren't fit for display) — backs Employee Card's "Goal".
        EmployeeStateId StateId { get; }

        // Display name of the zone this employee is currently moving to, working in, or was
        // stopped short of (see Stop/Continue below); empty when idle/returning to base/fleeing.
        // Backs Employee Card's "Destination" field.
        string DestinationName { get; }

        bool AssignTask(IEmployeeTask task);
        void Move(Vector3 point, Zone targetZone = null);

        // Halts in place but remembers whatever Move/AssignTask was in flight — Continue() resumes
        // it. Does not cancel/release it the way a fresh command would (see CancelCurrentTask).
        void Stop();

        // Resumes whatever Stop() interrupted. No-op if nothing was paused, and if the paused task
        // already finished/vanished on its own (e.g. another employee completed it) it's simply
        // dropped instead of resumed.
        void Continue();

        void ReturnToBase();

        void ApplyAttackOutcome(bool survived);
    }
}
