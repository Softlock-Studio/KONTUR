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

        private static void LogZone(string source, ZoneViewState zone)
        {
            string activities = string.Join(", ", zone.ActiveActivities);
            Debug.Log($"[HouseView] {source}: {zone.DisplayName} [{zone.RoomType}] " +
                      $"infection={zone.InfectionPercent:F1}% light={zone.HasLight} " +
                      $"slots={zone.FreeSlotCount}/{zone.SlotCount} selected={zone.IsSelected} " +
                      $"activities=[{activities}]");
        }
    }
}
