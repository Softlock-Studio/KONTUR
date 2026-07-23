namespace Game.House
{
    public sealed class ResolveZoneEventEffect : IZoneActivityEffect
    {
        private readonly ZoneEventType type;

        public ResolveZoneEventEffect(ZoneEventType type)
        {
            this.type = type;
        }

        public void Apply(Zone zone) => zone.ResolveEvent(type);
    }
}
