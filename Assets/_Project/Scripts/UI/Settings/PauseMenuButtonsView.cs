using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Settings
    {
    public class PauseMenuButtonsView : MonoBehaviour
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _backToMenuButton;

        private void Start()
        {
            if (_resumeButton == null)
            {
                Debug.LogError($"Resume button wasn't set in {gameObject.name}");
            }
            if (_settingsButton == null)
            {
                Debug.LogError($"Settings button wasn't set in {gameObject.name}");
            }
            if (_backToMenuButton == null)
            {
                Debug.LogError($"Back To Menu Button wasn't set in {gameObject.name}");
            }
        }

        public Button GetResumeButton() => _resumeButton;
        public Button GetSettingsButton() => _settingsButton;
        public Button GetBackToMenuButton() => _backToMenuButton;
    }
}