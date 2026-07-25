using System;

namespace Loader.SceneController
{
    [Serializable]
    public class SceneLoadContainer
    {
        public LevelType LevelType;
        [SceneDropdown] public string NameLevel;
    }
}