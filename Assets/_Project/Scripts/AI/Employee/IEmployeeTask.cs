using UnityEngine;

namespace Game.AI.Employee
{
    public interface IEmployeeTask
    {
        Vector3 TargetPosition { get; }
        float Duration { get; }

        void OnStarted();
        void OnCompleted();
        void OnCancelled();
    }
}
