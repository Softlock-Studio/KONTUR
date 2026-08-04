using Game.AI.Employee;
using Game.House;
using Game.House.Presentation;
using System;
using UnityEngine;

public interface IZoneActionMenuPresenter
{
    public void SubscribeToZoneClick(Action<Zone, Vector2> action);
    public void OpenZoneActionMenu(Zone zone, IEmployee employee, IHousePresenter housePresenter, Vector2 screenPosition);
}
