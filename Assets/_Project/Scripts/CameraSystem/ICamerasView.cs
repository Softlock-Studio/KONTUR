using System;
using System.Collections.Generic;

namespace CameraSystem
{
    public interface ICamerasView
    {
        public event Action<int> OnHandleClick;
        public void HandleClick(int cameraID);
        public void SelectCamera(int cameraID);
        public void ShowNoSignal();
        public List<GameCamera> GetCameraList();
        public float GetNoiseBlend(int cameraID);
    }
}