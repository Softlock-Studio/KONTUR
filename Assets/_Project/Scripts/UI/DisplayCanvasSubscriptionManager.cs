using Game.House.Presentation;
using Game.UI.Employees;
using System;
using UnityEngine;
using VContainer.Unity;

public class DisplayCanvasSubscriptionManager : IStartable, ITickable, IDisposable
{
    IHousePresenter _housePresenter;
    IEmployeeListPresenter _employeeListPresenter;
    IEmployeeActionButtonsPresenter _employeeActionButtonsPresenter;
    IZoneActionMenuPresenter _zoneActionMenuPresenter;

    public DisplayCanvasSubscriptionManager(IHousePresenter housePresenter, IEmployeeListPresenter employeeListPresenter, 
        IEmployeeActionButtonsPresenter actionButtonsPresenter, IZoneActionMenuPresenter zoneActionMenuPresenter)
    {
        _housePresenter = housePresenter;
        _employeeListPresenter = employeeListPresenter;
        _employeeActionButtonsPresenter = actionButtonsPresenter;
        _zoneActionMenuPresenter = zoneActionMenuPresenter;
    }

    public void Start()
    {
        _employeeListPresenter.SelectionChanged += _employeeActionButtonsPresenter.OnSelectionChanged;
        _employeeActionButtonsPresenter.OnSelectionChanged(_employeeListPresenter.SelectedEmployee);
        _employeeActionButtonsPresenter.BindMoveButtonClick(OnMoveClicked);
        _employeeActionButtonsPresenter.BindStopButtonClick(OnStopClicked);
        _employeeActionButtonsPresenter.BindReturnButtonClick(OnReturnClicked);

        _zoneActionMenuPresenter.SubscribeToZoneClick(OnZoneClicked);
    }

    private void OnZoneClicked(Game.House.Zone zone, Vector2 position)
    {
        _zoneActionMenuPresenter.OpenZoneActionMenu(zone, _employeeListPresenter.SelectedEmployee, _housePresenter, position);
    }
    private void OnMoveClicked() => _housePresenter.RequestContinueEmployee(_employeeListPresenter.SelectedEmployee);

    private void OnStopClicked() => _housePresenter.RequestStopEmployee(_employeeListPresenter.SelectedEmployee);

    private void OnReturnClicked() => _housePresenter.RequestReturnToBaseEmployee(_employeeListPresenter.SelectedEmployee);
    
    public void Dispose()
    {

    }

    public void Tick()
    {

    }
}
