namespace Game.House
{
    public sealed class RestoreLightEffect : IZoneActivityEffect
    {
        public void Apply(Zone zone) => zone.SetLight(true);
    }
}
