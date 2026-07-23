using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loader.SceneController
{
    public class SceneLoader
    {
        public async Task Load(string sceneName)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"[SceneLoader] '{sceneName}' is not enabled in Build Settings — add it via File > Build Settings (or the [SceneDropdown] warning in the Inspector). Load aborted.");
                return;
            }

            var newScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            newScene.allowSceneActivation = true;

            while(newScene.progress < 1)
                await Task.Yield();
        }

        public async Task Unload(string sceneName)
        {
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);

            if (loadedScene.IsValid())
                await SceneManager.UnloadSceneAsync(loadedScene);
        }
    }
}