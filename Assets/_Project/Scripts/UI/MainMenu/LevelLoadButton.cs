using Loader.SceneController;
using UnityEngine;

namespace Game.UI.MainMenu
{
    public class LevelLoadButton : MonoBehaviour
    {
        [SerializeField] MainMenuUI _mainMenu;
        [Space]
        [SerializeField] private LevelType _levelType;

        public void LoadLevel()
        {
            _mainMenu.LevelLoad(_levelType);
        }
    }
}