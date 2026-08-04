using System;
using UnityEngine;

namespace Loader.SceneController
{
    [Serializable]
    public class SceneLoadContainer
    {
        [Tooltip("Тип уровня/сцены (например, меню, миссия и т.д.).")]
        public LevelType LevelType;
        [Tooltip("Имя сцены для загрузки — выбирается из списка сцен, добавленных в Build Settings.")]
        [SceneDropdown] public string NameLevel;
    }
}
