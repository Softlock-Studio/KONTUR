using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.House
{
    public sealed class ZoneRegistry : MonoBehaviour, IInfectionDirector, IZoneDirectory
    {
        [SerializeField] private bool debugEnabled = false;

        private Zone[] zones;

        public IReadOnlyList<Zone> Zones => zones;

        private void Awake()
        {
            zones = FindObjectsByType<Zone>(FindObjectsSortMode.None);
        }

        public float GetInfectionLevel()
        {
            if (zones == null || zones.Length == 0) return 0f;
            return zones.Average(z => z.Infection) / 100f;
        }

        public IReadOnlyList<IWanderZone> GetZones() => (IReadOnlyList<IWanderZone>)zones ?? System.Array.Empty<IWanderZone>();

        public Zone FindZoneAt(Vector3 position)
        {
            if (zones == null) return null;

            foreach (Zone zone in zones)
                if (zone != null && zone.Contains(position)) return zone;

            return null;
        }

        private void OnGUI()
        {
            if (!debugEnabled || zones == null) return;

            GUI.Label(new Rect(520, 10, 300, 20), $"Zones (house avg: {GetInfectionLevel() * 100f:F1}%)");
            for (int i = 0; i < zones.Length; i++)
            {
                Zone zone = zones[i];
                if (zone == null) continue;

                string name = string.IsNullOrEmpty(zone.DisplayName) ? zone.name : zone.DisplayName;
                GUI.Label(new Rect(520, 30 + i * 20, 400, 20),
                    $"{name} [{zone.RoomType}]: {zone.Infection:F1}%   Light: {(zone.HasLight ? "On" : "Off")}   Free slots: {zone.FreeSlotCount}");
            }
        }
    }
}
