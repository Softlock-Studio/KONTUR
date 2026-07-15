using UnityEngine;

namespace Game.AI.Babooshka
{
    public interface IBabooshka
    {
        Vector3 Position { get; }
        string CurrentStateName { get; }
    }
}
