using System;
using System.Collections.Generic;
using UnityEngine;
using Game.AI.Employee;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.House
{
    public sealed class Zone : MonoBehaviour
    {
        [SerializeField] private RoomType roomType;
        [SerializeField] private string displayName;
        [SerializeField] private ZoneConfig config;
        [SerializeField] private Transform[] standingPoints;

        private IEmployee[] occupants;
        private ZoneTask[] reservingTask;
        private bool[] activityFinished;

        // Runtime-only; must not live on ZoneEventDefinition/ZoneConfig, since multiple Zone
        // instances commonly share the same ZoneConfig asset.
        private readonly Dictionary<ZoneEventType, float> eventCheckTimers = new();
        private readonly Dictionary<ZoneEventType, int> activeEventCounts = new();

        // Countdown(s) for currently-active occurrences that have ExpirySeconds configured.
        // Instant types can hold several entries at once (one per concurrent occurrence, up to
        // MaxConcurrent); Condition types hold at most one (the type is a boolean).
        private readonly Dictionary<ZoneEventType, List<float>> expiryTimers = new();

        private bool infectionOutbreakActive;

        public RoomType RoomType => roomType;
        public string DisplayName => displayName;
        public float Infection { get; private set; }
        public bool HasLight { get; private set; } = true;

        public event Action LightChanged;
        public event Action OccupancyChanged;
        public event Action ActivitiesChanged;
        public event Action EventsChanged;
        public event Action<ZoneEventType> EventExpired;

        public int SlotCount => occupants?.Length ?? 0;

        public int FreeSlotCount
        {
            get
            {
                int free = 0;
                for (int i = 0; i < occupants.Length; i++)
                    if (occupants[i] == null) free++;
                return free;
            }
        }

        private void Awake()
        {
            int count = standingPoints?.Length ?? 0;
            occupants = new IEmployee[count];
            reservingTask = new ZoneTask[count];
            activityFinished = new bool[count];
        }

        private void Update()
        {
            if (infectionOutbreakActive)
                Infection = Mathf.Clamp(Infection + GetGrowthRate() * Time.deltaTime, 0f, 100f);

            TickEvents(Time.deltaTime);
            TickExpiry(Time.deltaTime);
        }

        private void TickEvents(float deltaTime)
        {
            if (config.Events == null) return;

            foreach (ZoneEventDefinition definition in config.Events)
            {
                float timer = eventCheckTimers.TryGetValue(definition.Type, out float t)
                    ? t
                    : definition.CheckIntervalSeconds;

                timer -= deltaTime;
                if (timer > 0f)
                {
                    eventCheckTimers[definition.Type] = timer;
                    continue;
                }

                eventCheckTimers[definition.Type] = definition.CheckIntervalSeconds;

                if (!CanSpawn(definition)) continue;

                if (UnityEngine.Random.value <= definition.SpawnChance)
                    Spawn(definition);
            }
        }

        private bool CanSpawn(ZoneEventDefinition definition)
        {
            if (definition.Kind == ZoneEventKind.Condition)
                return !IsConditionActive(definition.Type);

            int activeCount = activeEventCounts.TryGetValue(definition.Type, out int c) ? c : 0;
            return activeCount < definition.MaxConcurrent;
        }

        private bool IsConditionActive(ZoneEventType type)
        {
            switch (type)
            {
                case ZoneEventType.LightOff: return !HasLight;
                case ZoneEventType.InfectionOutbreak: return infectionOutbreakActive;
                default: return false;
            }
        }

        private void Spawn(ZoneEventDefinition definition)
        {
            switch (definition.Type)
            {
                case ZoneEventType.LightOff:
                    SetLight(false);
                    return;

                case ZoneEventType.InfectionOutbreak:
                    TriggerInfectionOutbreak();
                    return;

                default:
                    int activeCount = activeEventCounts.TryGetValue(definition.Type, out int c) ? c : 0;
                    activeEventCounts[definition.Type] = activeCount + 1;
                    StartExpiryIfConfigured(definition.Type);
                    EventsChanged?.Invoke();
                    return;
            }
        }

        private ZoneEventDefinition FindEventDefinition(ZoneEventType type)
        {
            if (config.Events == null) return null;

            foreach (ZoneEventDefinition definition in config.Events)
                if (definition.Type == type) return definition;

            return null;
        }

        private void StartExpiryIfConfigured(ZoneEventType type)
        {
            ZoneEventDefinition definition = FindEventDefinition(type);
            if (definition == null || definition.ExpirySeconds <= 0f) return;

            if (!expiryTimers.TryGetValue(type, out List<float> list))
            {
                list = new List<float>();
                expiryTimers[type] = list;
            }

            list.Add(definition.ExpirySeconds);
        }

        private void TickExpiry(float deltaTime)
        {
            if (expiryTimers.Count == 0) return;

            foreach (var pair in expiryTimers)
            {
                List<float> list = pair.Value;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    list[i] -= deltaTime;
                    if (list[i] > 0f) continue;

                    list.RemoveAt(i);
                    HandleExpired(pair.Key);
                }
            }
        }

        private void HandleExpired(ZoneEventType type)
        {
            if (activeEventCounts.TryGetValue(type, out int count) && count > 0)
                activeEventCounts[type] = count - 1;

            EventExpired?.Invoke(type);
            EventsChanged?.Invoke();
        }

        // Ungated by ZoneConfig.Events on purpose: this is the hook a future grandmother AI
        // behavior can call directly on a zone she chooses, independent of that zone's own
        // ambient-spawn roster.
        public bool TriggerInfectionOutbreak()
        {
            if (infectionOutbreakActive) return false;

            infectionOutbreakActive = true;
            EventsChanged?.Invoke();
            StartExpiryIfConfigured(ZoneEventType.InfectionOutbreak);
            return true;
        }

        private float GetGrowthRate()
        {
            return config.BaseGrowthPerSecond + (HasLight ? 0f : config.DarknessGrowthPerSecond);
        }

        [ContextMenu("Toggle Light")]
        public void ToggleLight() => SetLight(!HasLight);

        public void SetLight(bool value)
        {
            if (value == HasLight) return;

            HasLight = value;
            LightChanged?.Invoke();

            if (!value) StartExpiryIfConfigured(ZoneEventType.LightOff);
            else expiryTimers.Remove(ZoneEventType.LightOff);
        }

        public void ReduceInfection(float amount)
        {
            Infection = Mathf.Clamp(Infection - amount, 0f, 100f);

            if (Infection <= 0f && infectionOutbreakActive)
            {
                infectionOutbreakActive = false;
                expiryTimers.Remove(ZoneEventType.InfectionOutbreak);
                EventsChanged?.Invoke();
            }
        }

        public IReadOnlyList<ActivityType> ActiveActivities
        {
            get
            {
                var active = new List<ActivityType>();
                for (int i = 0; i < reservingTask.Length; i++)
                    if (reservingTask[i] != null && !activityFinished[i]) active.Add(reservingTask[i].ActivityType);
                return active;
            }
        }

        public IReadOnlyList<ZoneEventType> ActiveEvents
        {
            get
            {
                var active = new List<ZoneEventType>();
                foreach (var pair in activeEventCounts)
                    for (int i = 0; i < pair.Value; i++)
                        active.Add(pair.Key);

                if (!HasLight) active.Add(ZoneEventType.LightOff);
                if (infectionOutbreakActive) active.Add(ZoneEventType.InfectionOutbreak);

                return active;
            }
        }

        public bool HasActiveEvent(ZoneEventType type)
            => activeEventCounts.TryGetValue(type, out int count) && count > 0;

        // No-ops if this type isn't currently active — correct for the race where an event
        // expires while an employee is mid-task resolving it.
        public void ResolveEvent(ZoneEventType type)
        {
            if (!activeEventCounts.TryGetValue(type, out int count) || count <= 0) return;

            activeEventCounts[type] = count - 1;

            if (expiryTimers.TryGetValue(type, out List<float> list) && list.Count > 0)
                list.RemoveAt(0);

            EventsChanged?.Invoke();
        }

        public bool TryAssign(IEmployee employee, ActivityType activityType, ZoneEventType? targetEvent,
            out string failureReason)
        {
            if (!TryBuildActivity(activityType, targetEvent, out ActivityDefinition activity, out failureReason))
                return false;

            int slotIndex = FindClaimableSlot(employee);
            if (slotIndex < 0)
            {
                failureReason = "No free standing point";
                return false;
            }

            var task = new ZoneTask(this, standingPoints[slotIndex].position, activity);
            occupants[slotIndex] = employee;
            reservingTask[slotIndex] = task;
            activityFinished[slotIndex] = false;

            if (!employee.AssignTask(task))
            {
                if (reservingTask[slotIndex] == task)
                {
                    occupants[slotIndex] = null;
                    reservingTask[slotIndex] = null;
                }

                failureReason = "Employee can't accept a command right now";
                return false;
            }

            OccupancyChanged?.Invoke();
            ActivitiesChanged?.Invoke();
            return true;
        }

        private bool TryBuildActivity(ActivityType type, ZoneEventType? targetEvent,
            out ActivityDefinition activity, out string failureReason)
        {
            failureReason = null;

            switch (type)
            {
                case ActivityType.Treatment:
                    activity = new ActivityDefinition(type, config.TreatmentDurationSeconds,
                        new ReduceInfectionEffect(config.TreatmentInfectionReduction));
                    return true;

                case ActivityType.LightbulbChange:
                    activity = new ActivityDefinition(type, config.LightbulbChangeDurationSeconds,
                        new RestoreLightEffect());
                    return true;

                case ActivityType.ResidentEvent:
                    if (!targetEvent.HasValue || !HasActiveEvent(targetEvent.Value))
                    {
                        activity = default;
                        failureReason = "No such active event in this zone";
                        return false;
                    }

                    activity = new ActivityDefinition(type, config.ResidentEventDurationSeconds,
                        new ResolveZoneEventEffect(targetEvent.Value));
                    return true;

                default:
                    activity = default;
                    failureReason = "Unknown activity type";
                    return false;
            }
        }

        private int FindClaimableSlot(IEmployee employee)
        {
            for (int i = 0; i < occupants.Length; i++)
                if (occupants[i] == null || occupants[i] == employee) return i;
            return -1;
        }

        // Marks the activity itself as done, independent of the slot reservation: the employee
        // keeps standing there, but it must stop showing as "active".
        internal void MarkActivityFinished(ZoneTask task)
        {
            for (int i = 0; i < reservingTask.Length; i++)
            {
                if (reservingTask[i] != task) continue;
                activityFinished[i] = true;
                ActivitiesChanged?.Invoke();
                return;
            }
        }

        internal void ReleaseSlot(ZoneTask task)
        {
            for (int i = 0; i < reservingTask.Length; i++)
            {
                if (reservingTask[i] != task) continue;
                occupants[i] = null;
                reservingTask[i] = null;
                activityFinished[i] = false;
                OccupancyChanged?.Invoke();
                ActivitiesChanged?.Invoke();
                return;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            string label = string.IsNullOrEmpty(displayName) ? name : displayName;
            Handles.Label(transform.position + Vector3.up * 2f,
                $"{label}\nInfection: {Infection:F1}%   Light: {(HasLight ? "On" : "Off")}");
        }
#endif
    }
}
