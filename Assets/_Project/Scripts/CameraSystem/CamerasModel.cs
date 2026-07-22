using System.Collections.Generic;
using Game.Audio;
using Game.House;
using UnityEngine;

namespace CameraSystem
{
    public enum CameraState
    {
        Enabled,
        Disabled
    }

    public class CamerasModel : ICameraObservationService
    {
        private readonly ZoneRegistry zoneRegistry;

        private Dictionary<int, CameraState> _cameraStates;
        private Dictionary<int, GameCamera> _camerasById;
        private int _selectedCameraID;

        public CamerasModel(ZoneRegistry zoneRegistry)
        {
            this.zoneRegistry = zoneRegistry;
        }

        public void Init(List<GameCamera> cameraList)
        {
            _cameraStates = new Dictionary<int, CameraState>();
            _camerasById = new Dictionary<int, GameCamera>();
            foreach (GameCamera cam in cameraList)
            {
                _cameraStates[cam.GetCameraID()] = CameraState.Enabled;
                _camerasById[cam.GetCameraID()] = cam;
            }
        }

        public void SelectCamera(int cameraID)
        {
            _selectedCameraID = cameraID;
        }

        public int GetSelectedCameraID() => _selectedCameraID;

        public void ChangeCameraState(int cameraID, CameraState state)
        {
            if (_cameraStates.ContainsKey(cameraID))
            {
                _cameraStates[cameraID] = state;
            }
        }

        // ICameraObservationService — used by the audio layer to decide whether a world sound
        // (footsteps, cleaning, growls, ...) is actually audible to the player right now.
        public bool IsObserving(Zone zone)
        {
            if (zone == null) return false;
            return GetSelectedZone() == zone;
        }

        public bool IsObserving(Vector3 worldPosition)
        {
            Zone zone = zoneRegistry != null ? zoneRegistry.FindZoneAt(worldPosition) : null;
            return IsObserving(zone);
        }

        private Zone GetSelectedZone()
        {
            if (_camerasById == null) return null;
            return _camerasById.TryGetValue(_selectedCameraID, out GameCamera camera) ? camera.GetObservedZone() : null;
        }
    }
}
