using System.Collections.Generic;

namespace CameraSystem
{
    public enum CameraState
    {
        Enabled,
        Disabled
    }

    public class CamerasModel
    {
        private Dictionary<int, CameraState> _cameraStates;
        private int _selectedCameraID;

        public void Init(List<GameCamera> cameraList)
        {
            _cameraStates = new Dictionary<int, CameraState>();
            foreach (GameCamera cam in cameraList)
            {
                _cameraStates[cam.GetCameraID()] = CameraState.Enabled;
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
    }
}
