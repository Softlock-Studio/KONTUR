using UnityEngine.SceneManagement;

namespace Loader.SceneController
{
    public class SceneLoader
    {
        public void Load(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        public void Unload(string sceneName)
        {
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);

            if (loadedScene.IsValid())
                SceneManager.UnloadSceneAsync(loadedScene);
        }
    }
}