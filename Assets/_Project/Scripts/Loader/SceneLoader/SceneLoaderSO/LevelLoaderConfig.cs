using System.Collections.Generic;
using UnityEngine;

namespace Loader.SceneController
{
    [CreateAssetMenu(menuName = "SceneLoader/Level Config", fileName = "LevelConfig")]
    public class LevelLoaderConfig : ScriptableObject
    {
        public List<SceneLoadContainer> Levels;
    }
}