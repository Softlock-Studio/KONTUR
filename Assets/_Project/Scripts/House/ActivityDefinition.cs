namespace Game.House
{
    public readonly struct ActivityDefinition
    {
        public ActivityType Type { get; }
        public float Duration { get; }
        public IZoneActivityEffect Effect { get; }

        public ActivityDefinition(ActivityType type, float duration, IZoneActivityEffect effect)
        {
            Type = type;
            Duration = duration;
            Effect = effect;
        }
    }
}
