namespace Game.House
{
    public readonly struct ActivityDefinition
    {
        public ActivityType Type { get; }
        public float Duration { get; }
        public IZoneActivityEffect Effect { get; }
        public ResourceType? ResourceType { get; }
        public int ResourceCost { get; }

        public ActivityDefinition(ActivityType type, float duration, IZoneActivityEffect effect,
            ResourceType? resourceType = null, int resourceCost = 0)
        {
            Type = type;
            Duration = duration;
            Effect = effect;
            ResourceType = resourceType;
            ResourceCost = resourceCost;
        }
    }
}
