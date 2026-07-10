using UnityEngine;

namespace Game.AI.Employee
{
    public interface IEmployee
    {
        Vector3 Position { get; }
        bool IsAlive { get; }
    }
}
