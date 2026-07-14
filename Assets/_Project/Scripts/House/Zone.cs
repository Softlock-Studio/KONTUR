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

        public RoomType RoomType => roomType;
        public string DisplayName => displayName;
        public float Infection { get; private set; }
        public bool HasLight { get; private set; } = true;

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
            occupants = new IEmployee[standingPoints?.Length ?? 0];
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
        public void ToggleLight()
        {
            HasLight = !HasLight;
        }

        public bool TryReserveSlot(IEmployee employee, out Transform slot)
        {
            for (int i = 0; i < occupants.Length; i++)
            {
                if (occupants[i] != null) continue;

                occupants[i] = employee;
                slot = standingPoints[i];
                return true;
            }

            slot = null;
            return false;
        }

        public void ReleaseSlot(IEmployee employee)
        {
            for (int i = 0; i < occupants.Length; i++)
            {
                if (occupants[i] != employee) continue;
                occupants[i] = null;
                return;
            }
        }

        public bool TryAssign(IEmployee employee, float taskDuration, out string failureReason)
        {
            failureReason = null;

            if (!TryReserveSlot(employee, out Transform slot))
            {
                failureReason = "No free standing point";
                return false;
            }

            var task = new ZoneTask(this, employee, slot.position, taskDuration);
            if (!employee.AssignTask(task))
            {
                ReleaseSlot(employee);
                failureReason = "Employee can't accept a command right now";
                return false;
            }

            return true;
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
