using Game.AI.Employee;
using Game.Mission;
using GLTF.Schema.KHR_lights_punctual;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI.Employees
{
    // Binds the fixed 5-slot Employee Card pool to whatever EmployeeRegistry finds in the scene,
    // and tracks which slot (if any) is currently selected for EmployeeActionButtonsView.
    public sealed class EmployeeListView : MonoBehaviour, IEmployeeListView
    {
        private const float GoalRefreshIntervalSeconds = 0.5f;

        [SerializeField] private EmployeeSlotView[] slots;

        private EmployeeSlotView selectedSlot;
        private float goalRefreshTimer;

        public event Action<IEmployee> SelectionChanged;
        private void Start()
        {
            var scope = LifetimeScope.Find<MissionScope>();
            EmployeeRegistry registry = scope.Container.Resolve<EmployeeRegistry>();

            BindSlots(registry.Employees);

            foreach (EmployeeSlotView slot in slots)
                slot.Clicked += OnSlotClicked;
        }

        private void OnDestroy()
        {
            if (slots == null) return;

            foreach (EmployeeSlotView slot in slots)
                if (slot != null) slot.Clicked -= OnSlotClicked;
        }

        private void Update()
        {
            goalRefreshTimer += Time.deltaTime;
            if (goalRefreshTimer < GoalRefreshIntervalSeconds) return;

            goalRefreshTimer = 0f;
            foreach (EmployeeSlotView slot in slots)
            {
                slot.RefreshGoal();
                slot.RefreshDestination();
            }
        }

        private void BindSlots(IReadOnlyList<IEmployee> employees)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (i < employees.Count) slots[i].Bind(employees[i]);
                else slots[i].SetEmpty();
            }
        }

        // Right-click-on-map also routes here (via MapClickController) so there's one place that
        // knows how to clear a selection.
        public void ClearSelection()
        {
            if (selectedSlot == null) return;

            selectedSlot.BoundEmployee.SetSelectionVisuals(false);
            selectedSlot.GetEmployeeCard().SetSelectedSprite(false);
            selectedSlot = null;

            SelectionChanged?.Invoke(null);
        }

        private void OnSlotClicked(EmployeeSlotView slot)
        {
            // Second left-click on the already-selected slot toggles selection off instead of
            // re-selecting the same employee.
            slot.GetEmployeeCard().PlayClickEffects();
            if (selectedSlot == slot)
            {
                ClearSelection();
                return;
            }
            if (selectedSlot != null)
            {
                selectedSlot.GetEmployeeCard().SetSelectedSprite(false);
                selectedSlot.BoundEmployee.SetSelectionVisuals(false);
            }
            
            selectedSlot = slot;
            slot.GetEmployeeCard().SetSelectedSprite(true);
            slot.BoundEmployee.SetSelectionVisuals(true);
            SelectionChanged?.Invoke(slot.BoundEmployee);
        }
    }
}
