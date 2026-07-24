using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Loader.SceneController
{
    public class SceneController
    {
        private bool _isDebug;

        private SceneLoader _sceneLoader;
        private Dictionary<LevelType, string> _sceneDictionary = new();

        private string _currentLevelScene = "MainMenu";

        public SceneController(LevelLoaderConfig config, bool isDebug = false)
        {
            _isDebug = isDebug;

            foreach (SceneLoadContainer container in config.Levels)
                _sceneDictionary[container.LevelType] = container.NameLevel;

            _sceneLoader = new SceneLoader();
        }

        public async void LevelLoad(LevelType sceneType)
        {
            if (IsValid(sceneType))
            {
                await SceneLoad(sceneType);

                if (_currentLevelScene != "")
                {
                    if (_isDebug)
                        Debug.Log("Start Unload " + _currentLevelScene);

                    await _sceneLoader.Unload(_currentLevelScene);
                }

                _currentLevelScene = _sceneDictionary[sceneType];
            }
        }

        public async Task SceneLoad(string sceneName)
        {
            if (_isDebug)
                Debug.Log("Start load " + sceneName);

            await _sceneLoader.Load(sceneName);
        }

        public async Task SceneLoad(LevelType sceneType)
        {
            if (IsValid(sceneType))
                await SceneLoad(_sceneDictionary[sceneType]);
        }

        private bool IsValid(LevelType sceneType)
        {
            return _sceneDictionary.ContainsKey(sceneType);
        }
    }
}