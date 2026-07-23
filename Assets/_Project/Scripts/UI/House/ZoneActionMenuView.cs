using System;
using System.Collections.Generic;
using Game.AI.Employee;
using Game.House;
using Game.House.Model;
using Game.House.Presentation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.House
{
    // Small popup listing only the actions actually valid for the clicked zone right now — the
    // GDD-specified "click a zone on the map -> context menu for task assignment" flow.
    public sealed class ZoneActionMenuView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Transform buttonParent;
        [SerializeField] private Button actionButtonPrefab;

        private readonly List<Button> spawnedButtons = new();

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        public void Open(Zone zone, IEmployee employee, IHousePresenter presenter)
        {
            if (zone == null || employee == null || presenter == null) return;

            Clear();

            ZoneId zoneId = ZoneId.From(zone);

            if (zone.Infection > 0f)
                AddAction("Treatment", () => presenter.RequestAssignTask(zoneId, employee, ActivityType.Treatment, null));

            if (!zone.HasLight)
                AddAction("Change lightbulb", () => presenter.RequestAssignTask(zoneId, employee, ActivityType.LightbulbChange, null));

            if (zone.HasActiveEvent(ZoneEventType.Emergency))
                AddAction("Resolve emergency", () => presenter.RequestAssignTask(zoneId, employee, ActivityType.ResidentEvent, ZoneEventType.Emergency));

            AddAction("Move here", () => presenter.RequestMoveEmployee(employee, zone.GetWanderPoint()));
            AddAction("Cancel", () => { });

            if (panelRoot != null) panelRoot.SetActive(true);
        }

        private void AddAction(string label, Action onClick)
        {
            Button button = Instantiate(actionButtonPrefab, buttonParent);

            TMP_Text text = button.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = label;

            button.onClick.AddListener(() =>
            {
                onClick();
                Close();
            });

            spawnedButtons.Add(button);
        }

        private void Close()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            Clear();
        }

        private void Clear()
        {
            foreach (Button button in spawnedButtons)
                if (button != null) Destroy(button.gameObject);

            spawnedButtons.Clear();
        }
    }
}
