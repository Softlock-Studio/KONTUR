using System;
using System.Collections.Generic;
using Game.House;
using Loader.SceneController;

namespace Game.Save
{
    [Serializable]
    public sealed class SaveData
    {
        public LevelType LevelType;
        public int AliveEmployeeCount;
        public List<ResourceCountEntry> ResourceCounts = new List<ResourceCountEntry>();
    }

    [Serializable]
    public struct ResourceCountEntry
    {
        public ResourceType Type;
        public int Count;
    }
}
