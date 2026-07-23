using System;
using Game.AI.Employee;
using Game.Bootstrap;
using Game.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

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

        private ILocalizationService localization;

        // ILocalizationService is game-wide (GameLifetimeScope), not mission-scoped — same
        // resolve pattern as SettingsPanelView. Lazy: Bind() can run from EmployeeListPresenter's
        // own Start(), before this component's Awake/Start would otherwise have cached it.
        private ILocalizationService Localization =>
            localization ??= LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<ILocalizationService>();

        public IEmployee BoundEmployee { get; private set; }

        public event Action<EmployeeSlotView> Clicked;

        public void Bind(IEmployee employee)
        {
            BoundEmployee = employee;

            if (unavailableRoot != null) unavailableRoot.SetActive(false);
            if (employeeCardRoot != null) employeeCardRoot.SetActive(true);

            var component = employee as Component;
            if (nameLabel != null) nameLabel.text = component != null ? component.name : Localization.Localize("Employee.Name.Fallback");

            // No per-employee portrait sprite exists anywhere yet (IEmployee has no such
            // accessor) — portrait is left as whatever the prefab authors, not overwritten here.

            RefreshGoal();
            RefreshDestination();
        }

        public void SetEmpty()
        {
            BoundEmployee = null;

            if (unavailableRoot != null) unavailableRoot.SetActive(true);
            if (employeeCardRoot != null) employeeCardRoot.SetActive(false);
        }

        // IEmployee has no target-zone/task-type accessor today — StateId is the closest available
        // approximation for the Card's "Goal" field (see plan's flagged gap).
        public void RefreshGoal()
        {
            if (BoundEmployee == null || goalText == null) return;
            goalText.text = Localization.Localize(GetStateKey(BoundEmployee.StateId));
        }

        private static string GetStateKey(EmployeeStateId state) => state switch
        {
            EmployeeStateId.Idle => "Employee.State.Idle",
            EmployeeStateId.MovingTo => "Employee.State.MovingTo",
            EmployeeStateId.PerformingTask => "Employee.State.PerformingTask",
            EmployeeStateId.ReturningToBase => "Employee.State.ReturningToBase",
            EmployeeStateId.Fleeing => "Employee.State.Fleeing",
            _ => "Employee.State.Idle",
        };

        public void RefreshDestination()
        {
            if (BoundEmployee == null || destinationText == null) return;

            string destination = BoundEmployee.DestinationName;
            destinationText.text = string.IsNullOrEmpty(destination) ? Localization.Localize("Employee.Destination.None") : destination;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (BoundEmployee != null) Clicked?.Invoke(this);
        }
    }
}
