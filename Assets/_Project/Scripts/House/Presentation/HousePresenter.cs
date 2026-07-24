using System;
using System.Collections.Generic;
using Game.AI.Employee;
using Game.House.Model;
using UnityEngine;
using VContainer.Unity;

namespace Game.House.Presentation
{
    public sealed class HousePresenter : IHousePresenter, IStartable, ITickable, IDisposable
    {
        private const float InfectionSampleIntervalSeconds = 0.5f;

        private readonly HouseModel model;
        private readonly IHouseView view;

        private ZoneId? selectedZoneId;
        private float sampleTimer;

        public HousePresenter(HouseModel model, IHouseView view)
        {
            this.model = model;
            this.view = view;
        }

        public void Start()
        {
            model.Initialize();
            model.ZoneChanged += OnZoneChanged;
            model.TaskFailed += OnTaskFailed;
            model.ResourceChanged += OnResourceChanged;
            model.ActivityAborted += OnActivityAborted;
            PushFullRender();
            view.RenderResources(model.GetAllResourceCounts());
        }

        public void Tick()
        {
            sampleTimer += Time.deltaTime;
            if (sampleTimer < InfectionSampleIntervalSeconds) return;

            sampleTimer = 0f;
            PushSample();
        }

        private void PushFullRender()
        {
            IReadOnlyList<ZoneSnapshot> snapshots = model.GetAllZoneSnapshots();
            var viewModels = new List<ZoneViewState>(snapshots.Count);
            foreach (ZoneSnapshot snapshot in snapshots)
                viewModels.Add(Map(snapshot));

            view.RenderZones(viewModels);
            view.SetHouseInfection(model.GetHouseInfectionLevel01());
        }

        private void PushSample()
        {
            foreach (ZoneSnapshot snapshot in model.GetAllZoneSnapshots())
                view.UpdateZone(Map(snapshot));

            view.SetHouseInfection(model.GetHouseInfectionLevel01());
        }

        private void OnZoneChanged(ZoneId id)
        {
            if (model.TryGetZoneSnapshot(id, out ZoneSnapshot snapshot))
                view.UpdateZone(Map(snapshot));
        }

        public void SelectZone(ZoneId zoneId)
        {
            selectedZoneId = zoneId;
            view.SetSelectedZone(selectedZoneId);
        }

        public void ClearSelection()
        {
            selectedZoneId = null;
            view.SetSelectedZone(null);
        }

        public void RequestAssignTask(ZoneId zoneId, IEmployee employee, ActivityType activityType,
            ZoneEventType? targetEvent)
        {
            if (employee == null) return;

            bool success = model.TryAssignTask(zoneId, employee, activityType, targetEvent, out string failureReason);
            view.ShowAssignmentResult(zoneId, success, failureReason);
        }

        private void OnTaskFailed(ZoneId zoneId, ZoneEventType eventType)
        {
            view.ShowTaskFailed(zoneId, eventType, model.FailedTaskCount);
        }

        private void OnResourceChanged(ResourceType type)
        {
            view.UpdateResource(type, model.GetResourceCount(type));
        }

        private void OnActivityAborted(ZoneId zoneId, ActivityType activityType, ResourceType resourceType)
        {
            view.ShowActivityAborted(zoneId, activityType, resourceType);
        }

        public void RequestStopEmployee(IEmployee employee) => employee?.Stop();

        public void RequestMoveEmployee(IEmployee employee, Zone zone)
        {
            if (employee == null || zone == null) return;
            employee.Move(zone.GetWanderPoint(), zone);
        }

        public void RequestContinueEmployee(IEmployee employee) => employee?.Continue();

        public void RequestReturnToBaseEmployee(IEmployee employee) => employee?.ReturnToBase();

        private ZoneViewState Map(ZoneSnapshot snapshot)
        {
            return new ZoneViewState(snapshot.Id, snapshot.DisplayName, snapshot.RoomType,
                snapshot.InfectionPercent, snapshot.HasLight, snapshot.FreeSlotCount, snapshot.SlotCount,
                isSelected: selectedZoneId.HasValue && selectedZoneId.Value == snapshot.Id,
                activeActivities: snapshot.ActiveActivities,
                activeEvents: snapshot.ActiveEvents);
        }

        public void Dispose()
        {
            model.ZoneChanged -= OnZoneChanged;
            model.TaskFailed -= OnTaskFailed;
            model.ResourceChanged -= OnResourceChanged;
            model.ActivityAborted -= OnActivityAborted;
        }
    }
}
