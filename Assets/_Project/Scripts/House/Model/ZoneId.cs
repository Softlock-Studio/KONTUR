using System;

namespace Game.House.Model
{
    public readonly struct ZoneId : IEquatable<ZoneId>
    {
        private readonly int value;

        private ZoneId(int value)
        {
            this.value = value;
        }

        internal static ZoneId From(Zone zone) => new ZoneId(zone.GetInstanceID());

        public bool Equals(ZoneId other) => value == other.value;
        public override bool Equals(object obj) => obj is ZoneId other && Equals(other);
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();

        public static bool operator ==(ZoneId left, ZoneId right) => left.Equals(right);
        public static bool operator !=(ZoneId left, ZoneId right) => !left.Equals(right);
    }
}
