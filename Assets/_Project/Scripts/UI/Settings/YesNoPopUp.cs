using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Settings
{
    public class YesNoPopUp : MonoBehaviour
    {
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;

        private void Start()
        {
            if (_yesButton == null)
                Debug.LogError($"Yes Button wasn't set in {gameObject.name}");

            if (_noButton == null)
                Debug.LogError($"No Button wasn't set in {gameObject.name}");
        }

        public Button GetYesButton() => _yesButton;
        public Button GetNoButton() => _noButton;
    }
}