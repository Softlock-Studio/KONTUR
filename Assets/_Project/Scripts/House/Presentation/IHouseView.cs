using System.Collections.Generic;
using Game.House.Model;

namespace Game.House.Presentation
{
    public interface IHouseView
    {
        void RenderZones(IReadOnlyList<ZoneViewState> zones);
        void UpdateZone(ZoneViewState zone);
        void SetHouseInfection(float infectionPercent01);
        void SetSelectedZone(ZoneId? selectedZoneId);
        void ShowAssignmentResult(ZoneId zoneId, bool success, string failureReason);
    }
}
