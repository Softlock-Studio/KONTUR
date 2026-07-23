using Game.Bootstrap;
using Loader.SceneController;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        private SceneController _sceneController;

        private void Start()
        {
            var scope = LifetimeScope.Find<GameLifetimeScope>();

            _sceneController = scope.Container.Resolve<SceneController>();
        }

        public void LevelLoad(LevelType level)
        {
            _sceneController.LevelLoad(level);
        }
    }
}