using System.Collections.Generic;
using UnityEngine;

namespace Loader.SceneController
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField] private bool _isDebug = false;

        private SceneLoader _sceneLoader;
        private Dictionary<LevelType, string> _sceneDictionary = new();

        private string _currentLevelScene = "";

        public void Initialize(LevelLoaderConfig config)
        {
            foreach (SceneLoadContainer container in config.Levels)
                _sceneDictionary[container.LevelType] = container.NameLevel;

            _sceneLoader = new SceneLoader();
        }

        public void LevelLoad(LevelType sceneType)
        {
            if (_currentLevelScene != "")
            {
                if (_isDebug)
                    Debug.Log("Start Unload " + _currentLevelScene);

                _sceneLoader.Unload(_currentLevelScene);
            }

            if (IsValid(sceneType))
            {
                SceneLoad(sceneType);
                _currentLevelScene = _sceneDictionary[sceneType];
            }
        }

        public void SceneLoad(string sceneName)
        {
            if (_isDebug)
                Debug.Log("Start load " + sceneName);

            _sceneLoader.Load(sceneName);
        }

        public void SceneLoad(LevelType sceneType)
        {
            if (IsValid(sceneType))
                SceneLoad(_sceneDictionary[sceneType]);
        }

        private bool IsValid(LevelType sceneType)
        {
            return _sceneDictionary.ContainsKey(sceneType);
        }
    }
}