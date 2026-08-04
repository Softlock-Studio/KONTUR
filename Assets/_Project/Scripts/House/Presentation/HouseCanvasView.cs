using Game.AI.Employee;
using Game.Bootstrap;
using Game.House.Model;
using Game.Mission;
using Game.UI.House;
using Game.UI.Settings;
using Infection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Game.House.Presentation
{
    // Real Canvas IHouseView, replacing DebugHouseConsoleView. No per-zone display surface exists
    // in Display Canvas yet (only the house-aggregate infection slider) — RenderZones/UpdateZone/
    // SetSelectedZone stay no-ops until that UI exists; see Docs/agents/systems/house.md.
    public sealed class HouseCanvasView : MonoBehaviour, IHouseView
    {
        [SerializeField] private InfectionGroup infectionGroup;
        [SerializeField] private ResourceGridPresenter resourceGrid;
        [SerializeField] private OrdersToastView ordersToast;
        [SerializeField] private ZoneMapLabelsPresenter zoneMapLabels;

        [SerializeField] private ReportView reportView;
        [Header("Pause")]
        [SerializeField] private Button openPauseMenuButton;
        [SerializeField] private PauseMenuView pauseMenuView;

        private MissionManager missionManager;

        private void Start()
        {
            missionManager = LifetimeScope.Find<MissionScope>().Container.Resolve<MissionManager>();
            missionManager.LevelEnded += OnLevelEnded;
            openPauseMenuButton.onClick.AddListener(pauseMenuView.ShowPauseMenu);
        }

        private void OnDestroy()
        {
            missionManager.LevelEnded -= OnLevelEnded;
            openPauseMenuButton.onClick.RemoveListener(pauseMenuView.ShowPauseMenu);
        }

        private void OnLevelEnded(LevelEndResult result) =>
            reportView.ShowReport(result.IsVictory, Mathf.RoundToInt(result.MaxInfectionReached01 * 100f), result.EmployeesKilled);

        // "Infection Label" (the mission's target infection corridor, e.g. floor/ceiling range)
        // is intentionally NOT wired here — the corridor system itself isn't built yet (see
        // Docs/agents/gdd/game-loop.md / map.md "Night cycle" row, still Planned), so there is no
        // live value to feed it. Leave its text authored by hand until that system exists.

        public void RenderZones(IReadOnlyList<ZoneViewState> zones) => zoneMapLabels?.Render(zones);

        public void UpdateZone(ZoneViewState zone) => zoneMapLabels?.UpdateItem(zone);

        public void SetSelectedZone(ZoneId? selectedZoneId)
        {
        }

        public void SetHouseInfection(float infectionPercent01)
        {
            if (infectionGroup != null)
            {
                infectionGroup.SetInfectionPercent(infectionPercent01);
            } 
        }

        public void SetHouseInfectionRange(float min, float max)
        {
            if (infectionGroup != null)
            {
                infectionGroup.SetInfectionRangeValues(min, max);
            }
        }

        public void ShowAssignmentResult(ZoneId zoneId, bool success, string failureReason)
        {
            ordersToast?.Show(success ? $"Assigned in {zoneId}" : $"Assign failed in {zoneId}: {failureReason}");
        }

        public void ShowTaskFailed(ZoneId zoneId, ZoneEventType eventType, int totalFailedCount)
        {
            ordersToast?.Show($"Task failed in {zoneId}: {eventType} (total: {totalFailedCount})");
        }

        public void RenderResources(IReadOnlyDictionary<ResourceType, int> counts) => resourceGrid?.Render(counts);

        public void UpdateResource(ResourceType type, int count) => resourceGrid?.UpdateItem(type, count);

        public void ShowActivityAborted(ZoneId zoneId, ActivityType activityType, ResourceType resourceType)
        {
            ordersToast?.Show($"{activityType} aborted in {zoneId}: not enough {resourceType}");
        }
    }
}
