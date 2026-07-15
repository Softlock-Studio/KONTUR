using System.Collections.Generic;

namespace Game.House.Model
{
    public readonly struct ZoneSnapshot
    {
        public ZoneId Id { get; }
        public string DisplayName { get; }
        public RoomType RoomType { get; }
        public float InfectionPercent { get; }
        public bool HasLight { get; }
        public int FreeSlotCount { get; }
        public int SlotCount { get; }
        public IReadOnlyList<ActivityType> ActiveActivities { get; }

        internal ZoneSnapshot(ZoneId id, string displayName, RoomType roomType, float infectionPercent,
            bool hasLight, int freeSlotCount, int slotCount, IReadOnlyList<ActivityType> activeActivities)
        {
            Id = id;
            DisplayName = displayName;
            RoomType = roomType;
            InfectionPercent = infectionPercent;
            HasLight = hasLight;
            FreeSlotCount = freeSlotCount;
            SlotCount = slotCount;
            ActiveActivities = activeActivities;
        }
    }
}
