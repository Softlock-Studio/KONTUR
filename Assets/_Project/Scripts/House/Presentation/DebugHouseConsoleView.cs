using System.Collections.Generic;
using Game.House.Model;
using UnityEngine;

namespace Game.House.Presentation
{
    // TEMPORARY remove once the real Canvas IHouseView implementation is registered.
    public sealed class DebugHouseConsoleView : MonoBehaviour, IHouseView
    {
        public void RenderZones(IReadOnlyList<ZoneViewState> zones)
        {
            Debug.Log($"[HouseView] RenderZones: {zones.Count} zone(s)");
            foreach (ZoneViewState zone in zones)
                LogZone("RenderZones", zone);   
        }

        public void UpdateZone(ZoneViewState zone) => LogZone("UpdateZone", zone);

        public void SetHouseInfection(float infectionPercent01)
        {
            Debug.Log($"[HouseView] House infection: {infectionPercent01 * 100f:F1}%");
        }

        public void SetSelectedZone(ZoneId? selectedZoneId)
        {
            Debug.Log($"[HouseView] Selected zone: {(selectedZoneId.HasValue ? selectedZoneId.Value.ToString() : "none")}");
        }

        public void ShowAssignmentResult(ZoneId zoneId, bool success, string failureReason)
        {
            Debug.Log($"[HouseView] Assign to zone {zoneId}: {(success ? "OK" : $"FAILED ({failureReason})")}");
        }

        public void ShowTaskFailed(ZoneId zoneId, ZoneEventType eventType, int totalFailedCount)
        {
            Debug.Log($"[HouseView] TASK FAILED in zone {zoneId}: {eventType} (total failed: {totalFailedCount})");
        }

        public void RenderResources(IReadOnlyDictionary<ResourceType, int> counts)
        {
            foreach (var pair in counts)
                Debug.Log($"[HouseView] RenderResources: {pair.Key} = {pair.Value}");
        }

        public void UpdateResource(ResourceType type, int count)
        {
            Debug.Log($"[HouseView] Resource {type} = {count}");
        }

        public void ShowActivityAborted(ZoneId zoneId, ActivityType activityType, ResourceType resourceType)
        {
            Debug.Log($"[HouseView] Activity {activityType} aborted in zone {zoneId}: not enough {resourceType}");
        }

        private static void LogZone(string source, ZoneViewState zone)
        {
            string activities = string.Join(", ", zone.ActiveActivities);
            string events = string.Join(", ", zone.ActiveEvents);
            Debug.Log($"[HouseView] {source}: {zone.DisplayName} [{zone.RoomType}] " +
                      $"infection={zone.InfectionPercent:F1}% light={zone.HasLight} " +
                      $"slots={zone.FreeSlotCount}/{zone.SlotCount} selected={zone.IsSelected} " +
                      $"activities=[{activities}] events=[{events}]");
        }
    }
}
