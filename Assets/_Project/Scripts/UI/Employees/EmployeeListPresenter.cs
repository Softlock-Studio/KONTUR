using Game.AI.Employee;
using System;
using VContainer.Unity;

namespace Game.UI.Employees
{
    public class EmployeeListPresenter : IEmployeeListPresenter, IStartable, IDisposable, ITickable
    {
        private IEmployeeListView _employeeListView;
        private IEmployee _selectedEmpployee;
        public IEmployee SelectedEmployee => _selectedEmpployee;
        public event Action<IEmployee> SelectionChanged;

        public EmployeeListPresenter(IEmployeeListView employeeListView)
        { 
            _employeeListView = employeeListView;
        }

        public void Start()
        {
            _employeeListView.SelectionChanged += employeeListView_SelectionChanged;
        }

        private void employeeListView_SelectionChanged(IEmployee employee)
        {
            _selectedEmpployee = employee;
            SelectionChanged?.Invoke(employee);
        }

        public void Dispose()
        {
            
        }

        public void Tick()
        {
            
        }
    }
}