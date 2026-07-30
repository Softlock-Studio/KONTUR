using Game.AI.Employee;
using UnityEngine.Events;

namespace Game.UI.Employees
{
    public interface IEmployeeActionButtonsPresenter
    {
        public void OnSelectionChanged(IEmployee employee);
        public void BindMoveButtonClick(UnityAction action);
        public void BindStopButtonClick(UnityAction action);
        public void BindReturnButtonClick(UnityAction action);
    }
}