using System.Collections.Generic;
using Game.House.Model;

namespace Game.House.Presentation
{
    public readonly struct ZoneViewState
    {
        public ZoneId Id { get; }
        public string DisplayName { get; }
        public RoomType RoomType { get; }
        public float InfectionPercent { get; }
        public bool HasLight { get; }
        public int FreeSlotCount { get; }
        public int SlotCount { get; }
        public bool IsSelected { get; }
        public IReadOnlyList<ActivityType> ActiveActivities { get; }
        public IReadOnlyList<ZoneEventType> ActiveEvents { get; }

        public ZoneViewState(ZoneId id, string displayName, RoomType roomType, float infectionPercent,
            bool hasLight, int freeSlotCount, int slotCount, bool isSelected,
            IReadOnlyList<ActivityType> activeActivities, IReadOnlyList<ZoneEventType> activeEvents)
        {
            Id = id;
            DisplayName = displayName;
            RoomType = roomType;
            InfectionPercent = infectionPercent;
            HasLight = hasLight;
            FreeSlotCount = freeSlotCount;
            SlotCount = slotCount;
            IsSelected = isSelected;
            ActiveActivities = activeActivities;
            ActiveEvents = activeEvents;
        }
    }
}
