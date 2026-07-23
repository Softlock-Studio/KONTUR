using System;
using Game.AI.Employee;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.House
{
    // One "Employee Slot" prefab instance in the fixed 5-slot pool under Employee Group. Toggles
    // between the "Unavailable" placeholder and the nested Employee Card depending on whether a
    // roster slot is bound.
    public sealed class EmployeeSlotView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject unavailableRoot;
        [SerializeField] private GameObject employeeCardRoot;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private Image portrait;
        [SerializeField] private TMP_Text goalText;
        [SerializeField] private TMP_Text destinationText;

        public IEmployee BoundEmployee { get; private set; }

        public event Action<EmployeeSlotView> Clicked;

        public void Bind(IEmployee employee)
        {
            BoundEmployee = employee;

            if (unavailableRoot != null) unavailableRoot.SetActive(false);
            if (employeeCardRoot != null) employeeCardRoot.SetActive(true);

            var component = employee as Component;
            if (nameLabel != null) nameLabel.text = component != null ? component.name : "Employee";

            // No per-employee portrait sprite exists anywhere yet (IEmployee has no such
            // accessor) — portrait is left as whatever the prefab authors, not overwritten here.

            // IEmployee has no target-zone/destination accessor today either.
            if (destinationText != null) destinationText.text = "-";

            RefreshGoal();
        }

        public void SetEmpty()
        {
            BoundEmployee = null;

            if (unavailableRoot != null) unavailableRoot.SetActive(true);
            if (employeeCardRoot != null) employeeCardRoot.SetActive(false);
        }

        // IEmployee has no target-zone/task-type accessor today — CurrentStateName is the closest
        // available approximation for the Card's "Goal" field (see plan's flagged gap).
        public void RefreshGoal()
        {
            if (BoundEmployee != null && goalText != null) goalText.text = BoundEmployee.CurrentStateName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (BoundEmployee != null) Clicked?.Invoke(this);
        }
    }
}
