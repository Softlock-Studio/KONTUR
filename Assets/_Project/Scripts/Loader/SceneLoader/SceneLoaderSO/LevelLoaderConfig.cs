using System.Collections.Generic;
using UnityEngine;

namespace Loader.SceneController
{
    [CreateAssetMenu(menuName = "SceneLoader/Level Config", fileName = "LevelConfig")]
    public class LevelLoaderConfig : ScriptableObject
    {
        [Tooltip("Список уровней (сцен), доступных для загрузки, с их типом и именем сцены.")]
        public List<SceneLoadContainer> Levels;
    }
}
