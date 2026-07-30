using Game.AI.Employee;
using System;
using UnityEngine.Events;
using VContainer.Unity;

namespace Game.UI.Employees
{
    public class EmployeeActionButtonsPresenter : IEmployeeActionButtonsPresenter, IStartable, ITickable, IDisposable
    {
        IEmployeeActionButtonsView _employeeActionButtonsView;

        public EmployeeActionButtonsPresenter(IEmployeeActionButtonsView employeeActionButtonsView)
        {
            _employeeActionButtonsView = employeeActionButtonsView;
        }

        public void OnSelectionChanged(IEmployee employee)
        {
            bool hasSelection = employee != null;
            _employeeActionButtonsView.SetActionButtonsInteractable(hasSelection);
        }

        public void BindMoveButtonClick(UnityAction action)
        {
            _employeeActionButtonsView.GetMoveButton().onClick.AddListener(action);
        }

        public void BindStopButtonClick(UnityAction action)
        {
            _employeeActionButtonsView.GetStopButton().onClick.AddListener(action);
        }

        public void BindReturnButtonClick(UnityAction action)
        {
            _employeeActionButtonsView.GetReturnButton().onClick.AddListener(action);
        }

        public void Start()
        {
        }

        public void Tick()
        {
        }

        public void Dispose()
        {
        }
    }
}