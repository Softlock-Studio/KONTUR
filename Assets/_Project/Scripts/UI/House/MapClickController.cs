using System;
using CameraSystem;
using Game.AI.Employee;
using Game.House;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI.House
{
    // On the Map RawImage. One raycast per click, sourced from that scene's top-down Map Camera:
    // a GameCamera hit switches the selected security camera feed; a Zone hit (with an employee
    // selected in the Employee List) opens the zone action menu — the GDD-specified "click
    // employee, click zone -> context menu" flow. There is no "click map to move" path any more;
    // "Move" (EmployeeActionButtonsView) resumes whatever was last given via the context menu
    // instead (see EmployeeController.Stop/Continue).
    public sealed class MapClickController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private MapUiConfig config;
        [SerializeField] private RectTransform mapRect;
        [SerializeField] private Camera mapCamera;
        [SerializeField] private CamerasView camerasView;
        [SerializeField] private EmployeeListPresenter employeeList;
        [SerializeField] private ZoneActionMenuView actionMenu;

        private void Start()
        {
            employeeList.SelectionChanged += OnSelectionChanged;
        }

        private void OnDestroy()
        {
            if (employeeList != null) employeeList.SelectionChanged -= OnSelectionChanged;
        }

        // Fires on any selection change — a fresh selection, a toggle-off in the Employee List, or
        // ClearSelection() below. Either way the previously-open action menu (built for whichever
        // employee was selected before) is stale.
        private void OnSelectionChanged(IEmployee employee)
        {
            actionMenu.Close();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Right-click anywhere on the map is a plain "cancel/deselect" — it never opens the
            // zone menu or moves anyone. Without this, a right-click near a zone would open the
            // menu right where you're about to click next (e.g. a camera icon), blocking it.
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                employeeList.ClearSelection();
                return;
            }

            Vector2 localClick;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRect, eventData.position, eventData.enterEventCamera, out localClick);
            //localClick.y = (textureRectTransform.rect.yMin * -1) - (localClick.y * -1);
            Debug.Log(localClick);
            Vector2 viewportClick = new Vector2(localClick.x / mapRect.rect.xMax, localClick.y / (mapRect.rect.yMin * -1));
            Debug.Log(viewportClick);
            viewportClick += new Vector2(1, 1);
            viewportClick /= 2;

            Vector3 worldClick = mapCamera.ViewportToWorldPoint(viewportClick);

            //if (!RawImageWorldRay.TryGetWorldRay(textureRectTransform, mapCamera, eventData.position, eventData.pressEventCamera, out Ray ray))
            //    return;

            // RaycastAll, not Raycast: the closest hit along this ray is often unrelated geometry
            // (floor, wall, another icon) sitting in front of the small camera-icon collider —
            // a single Raycast() would silently swallow the click on whatever's closest instead
            // of ever reaching the GameCamera. Look through every hit instead.
            RaycastHit[] hits = Physics.RaycastAll(worldClick, Vector3.down, config.MaxRayDistance, config.RaycastMask);

            // RaycastAll does not guarantee hit order. Both floors' Zone colliders sit in the same
            // XZ footprint and stay active regardless of FloorToggleView's camera-height hack (see
            // its comment — disabling the other floor would freeze its Zone.Update() simulation),
            // so an unsorted scan can land on the hidden floor's room instead of the one currently
            // shown. Sort by distance so "first match" means "nearest to the current camera", i.e.
            // the floor actually being viewed.
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

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

            actionMenu.Open(zone, employee, employeeList.HousePresenter, eventData.position);
        }
            
    }
}
