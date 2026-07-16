using System;
using System.Collections.Generic;
using Game.AI.Employee;

namespace Game.House.Model
{
    public sealed class HouseModel : IDisposable
    {
        private readonly ZoneRegistry zoneRegistry;
        private readonly ResourceInventory resources;
        private readonly Action<ResourceType> resourceChangedHandler;
        private readonly Dictionary<ZoneId, Zone> zonesById = new Dictionary<ZoneId, Zone>();
        private readonly Dictionary<Zone, Action> changeHandlers = new Dictionary<Zone, Action>();
        private readonly Dictionary<Zone, Action<ZoneEventType>> expiredHandlers = new Dictionary<Zone, Action<ZoneEventType>>();
        private readonly Dictionary<Zone, Action<ActivityType, ResourceType>> abortedHandlers = new Dictionary<Zone, Action<ActivityType, ResourceType>>();

        private bool initialized;

        public event Action<ZoneId> ZoneChanged;
        public event Action<ZoneId, ZoneEventType> TaskFailed;
        public event Action<ResourceType> ResourceChanged;
        public event Action<ZoneId, ActivityType, ResourceType> ActivityAborted;

        public int FailedTaskCount { get; private set; }

        public HouseModel(ZoneRegistry zoneRegistry, ResourceInventory resources)
        {
            this.zoneRegistry = zoneRegistry;
            this.resources = resources;
            resourceChangedHandler = type => ResourceChanged?.Invoke(type);
            resources.ResourceChanged += resourceChangedHandler;
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
                zone.EventsChanged += handler;

                Action<ZoneEventType> expiredHandler = type => OnZoneEventExpired(id, type);
                expiredHandlers[zone] = expiredHandler;
                zone.EventExpired += expiredHandler;

                Action<ActivityType, ResourceType> abortedHandler =
                    (activityType, resourceType) => ActivityAborted?.Invoke(id, activityType, resourceType);
                abortedHandlers[zone] = abortedHandler;
                zone.ActivityAborted += abortedHandler;

                zone.SetResourceProvider(resources);
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

        public int GetResourceCount(ResourceType type) => resources.GetCount(type);

        public IReadOnlyDictionary<ResourceType, int> GetAllResourceCounts() => resources.GetAllCounts();

        public void GrantResource(ResourceType type, int amount) => resources.Add(type, amount);

        public bool TryAssignTask(ZoneId zoneId, IEmployee employee, ActivityType activityType,
            ZoneEventType? targetEvent, out string failureReason)
        {
            if (!zonesById.TryGetValue(zoneId, out Zone zone))
            {
                failureReason = "Unknown zone";
                return false;
            }

            return zone.TryAssign(employee, activityType, targetEvent, out failureReason);
        }

        private void OnZoneEventExpired(ZoneId id, ZoneEventType type)
        {
            FailedTaskCount++;
            TaskFailed?.Invoke(id, type);
        }

        private static ZoneSnapshot BuildSnapshot(ZoneId id, Zone zone)
        {
            return new ZoneSnapshot(id, zone.DisplayName, zone.RoomType, zone.Infection,
                zone.HasLight, zone.FreeSlotCount, zone.SlotCount, zone.ActiveActivities, zone.ActiveEvents);
        }

        public void Dispose()
        {
            resources.ResourceChanged -= resourceChangedHandler;

            if (!initialized) return;

            foreach (var pair in changeHandlers)
            {
                Zone zone = pair.Key;
                if (zone == null) continue;
                zone.LightChanged -= pair.Value;
                zone.OccupancyChanged -= pair.Value;
                zone.ActivitiesChanged -= pair.Value;
                zone.EventsChanged -= pair.Value;
            }

            changeHandlers.Clear();

            foreach (var pair in expiredHandlers)
            {
                Zone zone = pair.Key;
                if (zone == null) continue;
                zone.EventExpired -= pair.Value;
            }

            expiredHandlers.Clear();

            foreach (var pair in abortedHandlers)
            {
                Zone zone = pair.Key;
                if (zone == null) continue;
                zone.ActivityAborted -= pair.Value;
            }

            abortedHandlers.Clear();
        }
    }
}
