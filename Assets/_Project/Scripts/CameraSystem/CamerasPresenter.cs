using System;
using VContainer.Unity;

namespace CameraSystem
{
    public class CamerasPresenter : IDisposable, IStartable, ITickable
    {
        ICamerasView _view;
        CamerasModel _model;

        public CamerasPresenter(ICamerasView view, CamerasModel model)
        { 
            _view = view;
            _model = model;
        }
        public void Start()
        {
            _model.Init(_view.GetCameraList());
            _view.OnHandleClick += OnCameraSelected;
            _view.ShowNoSignal();
        }
        public void Dispose()
        {
            _view.OnHandleClick -= OnCameraSelected;
        }

        public void OnCameraSelected(int cameraID)
        {
            _model.SelectCamera(cameraID);
            _view.SelectCamera(cameraID);
        }

        public void Tick()
        {
        }
    }
}