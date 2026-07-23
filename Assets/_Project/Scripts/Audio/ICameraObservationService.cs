using Game.House;
using UnityEngine;

namespace Game.Audio
{
    // Whether the player is currently watching (via the selected security camera) the zone a
    // sound originated in. Gates whether a world sound is actually audible to the player,
    // independent of the separate gameplay hearing/detection logic (HearingSensor), which always
    // runs regardless of what the player can hear.
    public interface ICameraObservationService
    {
        bool IsObserving(Zone zone);
        bool IsObserving(Vector3 worldPosition);
    }
}
