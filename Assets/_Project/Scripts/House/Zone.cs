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
            int count = standingPoints?.Length ?? 0;
            occupants = new IEmployee[count];
            reservingTask = new ZoneTask[count];
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

        public bool TryAssign(IEmployee employee, float taskDuration, out string failureReason)
        {
            failureReason = null;

            int slotIndex = FindClaimableSlot(employee);
            if (slotIndex < 0)
            {
                failureReason = "No free standing point";
                return false;
            }

            var task = new ZoneTask(this, standingPoints[slotIndex].position, taskDuration);
            occupants[slotIndex] = employee;
            reservingTask[slotIndex] = task;

            if (!employee.AssignTask(task))
            {
                // Only undo if nothing newer has already replaced this reservation.
                if (reservingTask[slotIndex] == task)
                {
                    occupants[slotIndex] = null;
                    reservingTask[slotIndex] = null;
                }

                failureReason = "Employee can't accept a command right now";
                return false;
            }

            return true;
        }

        private int FindClaimableSlot(IEmployee employee)
        {
            for (int i = 0; i < occupants.Length; i++)
                if (occupants[i] == null || occupants[i] == employee) return i;
            return -1;
        }

        internal void ReleaseSlot(ZoneTask task)
        {
            for (int i = 0; i < reservingTask.Length; i++)
            {
                if (reservingTask[i] != task) continue;
                occupants[i] = null;
                reservingTask[i] = null;
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
