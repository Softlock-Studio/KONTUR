using Game.AI.Employee;
using System;

namespace Game.UI.Employees
{
    public interface IEmployeeListPresenter
    {
        public event Action<IEmployee> SelectionChanged;
        public IEmployee SelectedEmployee => null;
    }
}