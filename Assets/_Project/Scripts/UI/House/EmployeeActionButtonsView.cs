using Game.AI.Employee;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.House
{
    // Move/Stop/Return buttons in Button Group. Move doesn't take a destination itself — it arms
    // MapClickController for one plain move on the next map click; Stop/Return act immediately.
    // All three are disabled while no employee is selected.
    public sealed class EmployeeActionButtonsView : MonoBehaviour
    {
        [SerializeField] private EmployeeListPresenter employeeList;
        [SerializeField] private MapClickController mapClickController;
        [SerializeField] private Button moveButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private Button returnButton;

        private void Awake()
        {
            if (moveButton != null) moveButton.onClick.AddListener(OnMoveClicked);
            if (stopButton != null) stopButton.onClick.AddListener(OnStopClicked);
            if (returnButton != null) returnButton.onClick.AddListener(OnReturnClicked);
        }

        private void Start()
        {
            employeeList.SelectionChanged += OnSelectionChanged;
            OnSelectionChanged(employeeList.SelectedEmployee);
        }

        private void OnDestroy()
        {
            if (employeeList != null) employeeList.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(IEmployee employee)
        {
            bool hasSelection = employee != null;
            if (moveButton != null) moveButton.interactable = hasSelection;
            if (stopButton != null) stopButton.interactable = hasSelection;
            if (returnButton != null) returnButton.interactable = hasSelection;
        }

        private void OnMoveClicked() => mapClickController.ArmPlainMove(employeeList.SelectedEmployee);

        private void OnStopClicked() => employeeList.HousePresenter.RequestStopEmployee(employeeList.SelectedEmployee);

        private void OnReturnClicked() => employeeList.HousePresenter.RequestReturnToBaseEmployee(employeeList.SelectedEmployee);
    }
}
