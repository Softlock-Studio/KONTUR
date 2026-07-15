namespace Game.House
{
    public sealed class ReduceInfectionEffect : IZoneActivityEffect
    {
        private readonly float amount;

        public ReduceInfectionEffect(float amount)
        {
            this.amount = amount;
        }

        public void Apply(Zone zone) => zone.ReduceInfection(amount);
    }
}
