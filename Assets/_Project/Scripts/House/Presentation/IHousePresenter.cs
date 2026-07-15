using Game.AI.Employee;
using Game.House.Model;
using UnityEngine;

namespace Game.House.Presentation
{
    public interface IHousePresenter
    {
        void SelectZone(ZoneId zoneId);
        void ClearSelection();
        void RequestAssignTask(ZoneId zoneId, IEmployee employee, ActivityType activityType);
        void RequestStopEmployee(IEmployee employee);
        void RequestMoveEmployee(IEmployee employee, Vector3 destination);
        void RequestReturnToBaseEmployee(IEmployee employee);
    }
}
