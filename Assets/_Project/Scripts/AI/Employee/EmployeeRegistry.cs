using System.Collections.Generic;
using UnityEngine;

namespace Game.AI.Employee
{
    public sealed class EmployeeRegistry : MonoBehaviour
    {
        private EmployeeController[] employees;

        public IReadOnlyList<IEmployee> Employees => employees;

        private void Awake()
        {
            employees = FindObjectsByType<EmployeeController>(FindObjectsSortMode.None);
        }
    }
}
