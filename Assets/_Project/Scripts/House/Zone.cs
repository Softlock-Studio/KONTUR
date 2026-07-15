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

        public RoomType RoomType => roomType;
        public string DisplayName => displayName;
        public float Infection { get; private set; }
        public bool HasLight { get; private set; } = true;

        public event Action LightChanged;
        public event Action OccupancyChanged;
        public event Action ActivitiesChanged;

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
            Infection = Mathf.Clamp(Infection + GetGrowthRate() * Time.deltaTime, 0f, 100f);
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
        }

        public void ReduceInfection(float amount)
        {
            Infection = Mathf.Clamp(Infection - amount, 0f, 100f);
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

        public bool TryAssign(IEmployee employee, ActivityType activityType, out string failureReason)
        {
            if (!TryBuildActivity(activityType, out ActivityDefinition activity, out failureReason))
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

        private bool TryBuildActivity(ActivityType type, out ActivityDefinition activity, out string failureReason)
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
