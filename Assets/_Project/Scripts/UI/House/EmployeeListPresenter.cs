using System;
using System.Collections.Generic;
using Game.AI.Employee;
using Game.House.Presentation;
using Game.Mission;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI.House
{
    // Binds the fixed 5-slot Employee Card pool to whatever EmployeeRegistry finds in the scene,
    // and tracks which slot (if any) is currently selected for EmployeeActionButtonsView.
    public sealed class EmployeeListPresenter : MonoBehaviour
    {
        private const float GoalRefreshIntervalSeconds = 0.5f;

        [SerializeField] private EmployeeSlotView[] slots;

        private IHousePresenter housePresenter;
        private EmployeeSlotView selectedSlot;
        private float goalRefreshTimer;

        public IEmployee SelectedEmployee => selectedSlot != null ? selectedSlot.BoundEmployee : null;
        public IHousePresenter HousePresenter => housePresenter;

        public event Action<IEmployee> SelectionChanged;

        private void Start()
        {
            var scope = LifetimeScope.Find<MissionScope>();
            EmployeeRegistry registry = scope.Container.Resolve<EmployeeRegistry>();
            housePresenter = scope.Container.Resolve<IHousePresenter>();

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
                slot.RefreshGoal();
        }

        private void BindSlots(IReadOnlyList<IEmployee> employees)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (i < employees.Count) slots[i].Bind(employees[i]);
                else slots[i].SetEmpty();
            }
        }

        private void OnSlotClicked(EmployeeSlotView slot)
        {
            selectedSlot = slot;
            SelectionChanged?.Invoke(slot.BoundEmployee);
        }
    }
}
