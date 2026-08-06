using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loader.SceneController
{
    public class SceneController
    {
        private bool _isDebug;

        private SceneLoader _sceneLoader;
        private Dictionary<LevelType, string> _sceneDictionary = new();

        private string _currentLevelScene = "";
        private LevelType _currentLevelType;

        public string CurrentLevel => _currentLevelScene;
        public LevelType GetCurrentLevelType() => _currentLevelType;

        public SceneController(LevelLoaderConfig config, bool isDebug = false)
        {
            _isDebug = isDebug;

            foreach (SceneLoadContainer container in config.Levels)
                _sceneDictionary[container.LevelType] = container.NameLevel;

            _sceneLoader = new SceneLoader();

            _currentLevelScene = SceneManager.GetActiveScene().name;
        }

        public async void LevelLoad(LevelType sceneType)
        {
            if (IsValid(sceneType))
            {
                // Load before unload, not the other way around: exactly one level scene is ever
                // loaded in this architecture, and SceneManager.UnloadSceneAsync returns null
                // (Unity refuses to unload the only loaded scene) if there's nothing else loaded
                // yet — unloading first threw a NullReferenceException on the very first
                // transition. The resulting brief overlap (both scenes loaded at once) is handled
                // by making MissionScope lookups scene-aware instead — see LifetimeScope.Find
                // call sites using the Scene overload.
                await SceneLoad(sceneType);

                if (_currentLevelScene != "")
                {
                    if (_isDebug)
                        Debug.Log("Start Unload " + _currentLevelScene);

                    await _sceneLoader.Unload(_currentLevelScene);
                }

                _currentLevelScene = _sceneDictionary[sceneType];
                _currentLevelType = sceneType;
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