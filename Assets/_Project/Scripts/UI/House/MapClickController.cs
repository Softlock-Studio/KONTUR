using CameraSystem;
using Game.AI.Employee;
using Game.House;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI.House
{
    // On the Map RawImage. One raycast per click, sourced from that scene's top-down Map Camera:
    // a GameCamera hit switches the selected security camera feed; a Zone hit (with an employee
    // selected in the Employee List) either performs an armed plain move or opens the zone action
    // menu — see the plan for why this isn't a raw "move to any point" raycast.
    public sealed class MapClickController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private MapUiConfig config;
        [SerializeField] private RectTransform mapRect;
        [SerializeField] private Camera mapCamera;
        [SerializeField] private CamerasView camerasView;
        [SerializeField] private EmployeeListPresenter employeeList;
        [SerializeField] private ZoneActionMenuView actionMenu;

        private IEmployee armedMoveEmployee;

        public void ArmPlainMove(IEmployee employee) => armedMoveEmployee = employee;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!RawImageWorldRay.TryGetWorldRay(mapRect, mapCamera, eventData.position, eventData.pressEventCamera, out Ray ray))
                return;

            // RaycastAll, not Raycast: the closest hit along this ray is often unrelated geometry
            // (floor, wall, another icon) sitting in front of the small camera-icon collider —
            // a single Raycast() would silently swallow the click on whatever's closest instead
            // of ever reaching the GameCamera. Look through every hit instead.
            RaycastHit[] hits = Physics.RaycastAll(ray, config.MaxRayDistance, config.RaycastMask);

            GameCamera camera = null;
            Zone zone = null;
            foreach (RaycastHit hit in hits)
            {
                if (camera == null) camera = hit.collider.GetComponentInParent<GameCamera>();
                if (zone == null) zone = hit.collider.GetComponentInParent<Zone>();
            }

            if (camera != null)
            {
                camerasView.HandleClick(camera.GetCameraID());
                return;
            }

            if (zone == null) return;

            IEmployee employee = employeeList.SelectedEmployee;
            if (employee == null) return;

            if (armedMoveEmployee != null)
            {
                employeeList.HousePresenter.RequestMoveEmployee(armedMoveEmployee, zone.GetWanderPoint());
                armedMoveEmployee = null;
                return;
            }

            actionMenu.Open(zone, employee, employeeList.HousePresenter);
        }
    }
}
