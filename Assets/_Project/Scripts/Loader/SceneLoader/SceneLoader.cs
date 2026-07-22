using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loader.SceneController
{
    public class SceneLoader
    {
        public async Task Load(string sceneName)
        {
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