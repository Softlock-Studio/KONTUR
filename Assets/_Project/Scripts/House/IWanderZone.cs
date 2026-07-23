using System.Collections.Generic;
using UnityEngine;

namespace Game.House
{
    // A single point Babooshka's WanderState can wander into and mess with.
    public interface IWanderZone
    {
        bool IsApartment { get; }
        Vector3 GetWanderPoint();
        bool TriggerInfectionOutbreak();
    }

    public interface IZoneDirectory
    {
        IReadOnlyList<IWanderZone> GetZones();
    }
}
