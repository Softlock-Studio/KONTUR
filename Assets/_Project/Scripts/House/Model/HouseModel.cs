using System;
using System.Collections.Generic;
using Game.AI.Employee;

namespace Game.House.Model
{
    public sealed class HouseModel : IDisposable
    {
        private readonly ZoneRegistry zoneRegistry;
        private readonly Dictionary<ZoneId, Zone> zonesById = new Dictionary<ZoneId, Zone>();
        private readonly Dictionary<Zone, Action> changeHandlers = new Dictionary<Zone, Action>();

        private bool initialized;

        public event Action<ZoneId> ZoneChanged;

        public HouseModel(ZoneRegistry zoneRegistry)
        {
            this.zoneRegistry = zoneRegistry;
        }

        public void Initialize()
        {
            if (initialized) return;
            initialized = true;

            foreach (Zone zone in zoneRegistry.Zones)
            {
                ZoneId id = ZoneId.From(zone);
                zonesById[id] = zone;

                Action handler = () => ZoneChanged?.Invoke(id);
                changeHandlers[zone] = handler;

                zone.LightChanged += handler;
                zone.OccupancyChanged += handler;
                zone.ActivitiesChanged += handler;
            }
        }

        public IReadOnlyList<ZoneSnapshot> GetAllZoneSnapshots()
        {
            var snapshots = new List<ZoneSnapshot>(zonesById.Count);
            foreach (var pair in zonesById)
                snapshots.Add(BuildSnapshot(pair.Key, pair.Value));
            return snapshots;
        }

        public bool TryGetZoneSnapshot(ZoneId id, out ZoneSnapshot snapshot)
        {
            if (zonesById.TryGetValue(id, out Zone zone))
            {
                snapshot = BuildSnapshot(id, zone);
                return true;
            }

            snapshot = default;
            return false;
        }

        public float GetHouseInfectionLevel01() => zoneRegistry.GetInfectionLevel();

        public bool TryAssignTask(ZoneId zoneId, IEmployee employee, ActivityType activityType, out string failureReason)
        {
            if (!zonesById.TryGetValue(zoneId, out Zone zone))
            {
                failureReason = "Unknown zone";
                return false;
            }

            return zone.TryAssign(employee, activityType, out failureReason);
        }

        private static ZoneSnapshot BuildSnapshot(ZoneId id, Zone zone)
        {
            return new ZoneSnapshot(id, zone.DisplayName, zone.RoomType, zone.Infection,
                zone.HasLight, zone.FreeSlotCount, zone.SlotCount, zone.ActiveActivities);
        }

        public void Dispose()
        {
            if (!initialized) return;

            foreach (var pair in changeHandlers)
            {
                Zone zone = pair.Key;
                if (zone == null) continue;
                zone.LightChanged -= pair.Value;
                zone.OccupancyChanged -= pair.Value;
                zone.ActivitiesChanged -= pair.Value;
            }

            changeHandlers.Clear();
        }
    }
}
