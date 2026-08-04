using Game.AI.Employee;
using Game.House;
using Game.House.Presentation;
using System;
using UnityEngine;

public interface IZoneActionMenuView
{
    public event Action<Zone, Vector2> OnZoneClick;
    public void Open(Zone zone, IEmployee employee, IHousePresenter presenter, Vector2 screenPosition);
}
