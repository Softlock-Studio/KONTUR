using UnityEngine;

namespace Game.AI.Employee
{
    public interface IEmployee
    {
        Vector3 Position { get; }
        bool IsAlive { get; }
        string CurrentStateName { get; }

        bool AssignTask(IEmployeeTask task);
        void Move(Vector3 point);
        void Stop();
        void ReturnToBase();

        void ApplyAttackOutcome(bool survived);
    }
}
