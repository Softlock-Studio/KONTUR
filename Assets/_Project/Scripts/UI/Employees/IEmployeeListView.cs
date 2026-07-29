using Game.AI.Employee;
using System;

public interface IEmployeeListView
{
    public event Action<IEmployee> SelectionChanged;
}
