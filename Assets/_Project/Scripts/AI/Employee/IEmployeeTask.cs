using UnityEngine;

namespace Game.AI.Employee
{
    public interface IEmployeeTask
    {
        Vector3 TargetPosition { get; }

        // Polled every tick while PerformingTask is active. Not a fixed duration on purpose —
        // e.g. ZoneTask's completion depends on a shared, dynamically-speeding-up progress pool.
        bool IsComplete { get; }

        // Null = perform this task silently (no periodic hearing/SFX pulse while active).
        EmployeeSoundType? AmbientSoundType { get; }

        // Return false to abort right here (e.g. a resource ran out between assignment and
        // arrival) — the caller treats this exactly like an external cancellation.
        bool OnStarted();
        void OnCompleted();
        void OnCancelled();
    }
}
