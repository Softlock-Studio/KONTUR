using Game.AI.Employee;
using Game.House.Model;
using UnityEngine;

namespace Game.House.Presentation
{
    public interface IHousePresenter
    {
        void SelectZone(ZoneId zoneId);
        void ClearSelection();
        void RequestAssignTask(ZoneId zoneId, IEmployee employee, ActivityType activityType, ZoneEventType? targetEvent);
        void RequestStopEmployee(IEmployee employee);
        void RequestMoveEmployee(IEmployee employee, Zone zone);
        void RequestReturnToBaseEmployee(IEmployee employee);
    }
}
