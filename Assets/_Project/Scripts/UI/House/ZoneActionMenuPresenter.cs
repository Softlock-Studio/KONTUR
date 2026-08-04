using Game.AI.Employee;
using Game.House;
using Game.House.Presentation;
using System;
using UnityEngine;
using VContainer.Unity;

public class ZoneActionMenuPresenter : IZoneActionMenuPresenter, IStartable, ITickable, IDisposable
{
    IZoneActionMenuView _zoneActionMenuView;

    public ZoneActionMenuPresenter(IZoneActionMenuView zoneActionMenuView)
    {
        _zoneActionMenuView = zoneActionMenuView;
    }

    public void SubscribeToZoneClick(Action<Zone, Vector2> action)
    {
        _zoneActionMenuView.OnZoneClick += action;
    }

    public void OpenZoneActionMenu(Zone zone, IEmployee employee, IHousePresenter housePresenter, Vector2 screenPosition)
    {
        _zoneActionMenuView.Open(zone, employee, housePresenter, screenPosition);
    }

    public void Start()
    {
    }

    public void Tick()
    {
    }

    public void Dispose()
    {
    }
}
