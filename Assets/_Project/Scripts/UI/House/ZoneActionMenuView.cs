using System;
using System.Collections.Generic;
using Game.AI.Employee;
using Game.Bootstrap;
using Game.House;
using Game.House.Model;
using Game.House.Presentation;
using Game.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Game.UI.House
{
    // Small popup listing only the actions actually valid for the clicked zone right now — the
    // GDD-specified "click a zone on the map -> context menu for task assignment" flow.
    public sealed class ZoneActionMenuView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Transform buttonParent;
        [SerializeField] private Button actionButtonPrefab;

        private readonly List<Button> spawnedButtons = new();

        private RectTransform panelRect;
        private Canvas rootCanvas;
        private ILocalizationService localization;

        // ILocalizationService is game-wide (GameLifetimeScope), not mission-scoped — same
        // resolve pattern as SettingsPanelView/EmployeeSlotView.
        private ILocalizationService Localization =>
            localization ??= LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<ILocalizationService>();

        private void Awake()
        {
            panelRect = panelRoot != null ? panelRoot.GetComponent<RectTransform>() : null;

            Canvas canvas = GetComponentInParent<Canvas>();
            rootCanvas = canvas != null ? canvas.rootCanvas : null;

            if (panelRoot != null) panelRoot.SetActive(false);
        }

        public void Open(Zone zone, IEmployee employee, IHousePresenter presenter, Vector2 screenPosition)
        {
            if (zone == null || employee == null || presenter == null) return;

            Clear();

            ZoneId zoneId = ZoneId.From(zone);

            if (zone.Infection > 0f)
                AddAction(Localization.Localize("ZoneMenu.Treatment"), () => presenter.RequestAssignTask(zoneId, employee, ActivityType.Treatment, null));

            if (!zone.HasLight)
                AddAction(Localization.Localize("ZoneMenu.ChangeLightbulb"), () => presenter.RequestAssignTask(zoneId, employee, ActivityType.LightbulbChange, null));

            if (zone.HasActiveEvent(ZoneEventType.Emergency))
                AddAction(Localization.Localize("ZoneMenu.ResolveEmergency"), () => presenter.RequestAssignTask(zoneId, employee, ActivityType.ResidentEvent, ZoneEventType.Emergency));

            AddAction(Localization.Localize("ZoneMenu.MoveHere"), () => presenter.RequestMoveEmployee(employee, zone));
            AddAction(Localization.Localize("ZoneMenu.Cancel"), () => { });

            if (panelRoot != null) panelRoot.SetActive(true);

            PositionAt(screenPosition);
        }

        // Places the panel's pivot at the click point in world space (anchor/hierarchy-agnostic —
        // works no matter how panelRect is anchored/parented under the canvas), then nudges it back
        // onto the screen if the click was near an edge/corner and the panel would otherwise stick
        // out. All math goes through RectTransformUtility so it stays correct under any Canvas
        // render mode / CanvasScaler setting, not just pixel-for-pixel Screen Space Overlay.
        private void PositionAt(Vector2 screenPosition)
        {
            if (panelRect == null || rootCanvas == null) return;

            RectTransform canvasRect = rootCanvas.transform as RectTransform;
            if (canvasRect == null) return;

            Camera cam = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, screenPosition, cam, out Vector3 worldPoint))
                panelRect.position = worldPoint;

            // Buttons were just (re)spawned — force an immediate layout pass so panelRect's rect
            // reflects its real size before we clamp against the screen bounds.
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);

            ClampToScreen(canvasRect, cam);
        }

        private void ClampToScreen(RectTransform canvasRect, Camera cam)
        {
            var corners = new Vector3[4];
            panelRect.GetWorldCorners(corners); // [0] bottom-left, [2] top-right

            Vector2 min = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
            Vector2 max = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);

            float shiftX = 0f;
            if (max.x > Screen.width) shiftX = Screen.width - max.x;
            if (min.x + shiftX < 0f) shiftX = -min.x; // panel wider than the screen: keep the left edge visible

            float shiftY = 0f;
            if (max.y > Screen.height) shiftY = Screen.height - max.y;
            if (min.y + shiftY < 0f) shiftY = -min.y; // panel taller than the screen: keep the bottom edge visible

            if (shiftX == 0f && shiftY == 0f) return;

            Vector2 shiftedScreenPoint = RectTransformUtility.WorldToScreenPoint(cam, panelRect.position) + new Vector2(shiftX, shiftY);
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, shiftedScreenPoint, cam, out Vector3 shiftedWorldPoint))
                panelRect.position = shiftedWorldPoint;
        }

        private void AddAction(string label, Action onClick)
        {
            Button button = Instantiate(actionButtonPrefab, buttonParent);

            TMP_Text text = button.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = label;

            button.onClick.AddListener(() =>
            {
                onClick();
                Close();
            });

            spawnedButtons.Add(button);
        }

        // Public: MapClickController also closes this when the employee selection changes (e.g.
        // right-click-to-deselect on the map), since a stale menu would otherwise sit in front of
        // the cursor and swallow the next click.
        public void Close()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            Clear();
        }

        private void Clear()
        {
            foreach (Button button in spawnedButtons)
                if (button != null) Destroy(button.gameObject);

            spawnedButtons.Clear();
        }
    }
}
