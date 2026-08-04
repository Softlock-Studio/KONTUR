using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Employees
{
    // Move/Stop/Return buttons in Button Group. "Move" is really Continue/Resume: Stop halts the
    // employee in place without abandoning whatever Move/AssignTask was in flight (see
    // EmployeeController.Stop), and this button resumes exactly that — it does not pick a new
    // destination (that's the zone context menu's job, see ZoneActionMenuView). All three are
    // disabled while no employee is selected.
    public sealed class EmployeeActionButtonsView : MonoBehaviour, IEmployeeActionButtonsView
    {
        [SerializeField] private Button _moveButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _returnButton;

        private void Awake()
        {
            if (_stopButton == null)
                Debug.LogError($"Stop button wasnt' set in {gameObject.name}");
            if (_moveButton == null)
                Debug.LogError($"Move button wasnt' set in {gameObject.name}");
            if (_returnButton == null)
                Debug.LogError($"Stop button wasnt' set in {gameObject.name}");
        }

        public void SetActionButtonsInteractable(bool val)
        {
            _moveButton.interactable = val;
            _stopButton.interactable = val;
            _returnButton.interactable = val;
        }

        public Button GetMoveButton() => _moveButton;
        public Button GetStopButton() => _stopButton;
        public Button GetReturnButton() => _returnButton;
    }
}
