using System;
using System.Collections.Generic;

namespace Game.House.Model
{
    public sealed class ResourceInventory : IResourceProvider
    {
        private readonly Dictionary<ResourceType, int> counts = new Dictionary<ResourceType, int>();

        public event Action<ResourceType> ResourceChanged;

        public ResourceInventory(ResourceConfig config)
        {
            counts[ResourceType.Iodine] = config.StartingIodine;
            counts[ResourceType.Lightbulb] = config.StartingLightbulbs;
        }

        public int GetCount(ResourceType type) => counts.TryGetValue(type, out int c) ? c : 0;

        public IReadOnlyDictionary<ResourceType, int> GetAllCounts()
            => new Dictionary<ResourceType, int>(counts);

        public bool TrySpend(ResourceType type, int amount)
        {
            int current = GetCount(type);
            if (current < amount) return false;

            counts[type] = current - amount;
            ResourceChanged?.Invoke(type);
            return true;
        }

        public void Add(ResourceType type, int amount)
        {
            if (amount <= 0) return;

            counts[type] = GetCount(type) + amount;
            ResourceChanged?.Invoke(type);
        }
    }
}
