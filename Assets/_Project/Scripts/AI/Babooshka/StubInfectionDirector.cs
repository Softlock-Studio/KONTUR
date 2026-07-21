using UnityEngine;
using Game.House;

namespace Game.AI.Babooshka
{
    public sealed class StubInfectionDirector : MonoBehaviour, IInfectionDirector
    {
        [SerializeField, Range(0f, 1f)] private float infectionLevel = 0.5f;

        public float GetInfectionLevel() => infectionLevel;
    }
}
