using CameraSystem;
using Game.AI.Employee;
using Game.Audio;
using Game.House;
using Game.House.Model;
using Game.House.Presentation;
using VContainer;
using VContainer.Unity;
using UnityEngine;
using Game.UI.House;
using Game.UI.Employees;

namespace Game.Mission
{
    public sealed class MissionScope : LifetimeScope
    {
        [SerializeField] private HouseConfig houseConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(houseConfig);

            builder.RegisterComponentInHierarchy<ZoneRegistry>();
            builder.RegisterComponentInHierarchy<EmployeeRegistry>();

            builder.Register<HouseModel>(Lifetime.Scoped);

            builder.RegisterEntryPoint<HousePresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<MissionManager>(Lifetime.Scoped).As<ITickable>().AsSelf();

            // Requires a HouseCanvasView somewhere in the scene (added via the AgentTools scene-edit
            // menu item, or manually) — RegisterComponentInHierarchy throws at container-build if
            // none exists, same class of bug as the BackgroundMusicTrigger incident earlier.
            builder.RegisterComponentInHierarchy<HouseCanvasView>().As<IHouseView>();

            // Also exposed as ICameraObservationService — AudioEmitter (scene-level, injected via
            // this same scope) uses it to decide whether a world sound is currently audible to the
            // player (only through the selected camera's room), not registered in GameLifetimeScope
            // since it depends on ZoneRegistry, which is mission-scoped.
            builder.RegisterComponentInHierarchy<CamerasView>().As<ICamerasView>();
            builder.Register<CamerasModel>(Lifetime.Scoped).AsSelf().As<ICameraObservationService>();
            builder.RegisterEntryPoint<CamerasPresenter>(Lifetime.Scoped);

            builder.RegisterEntryPoint<ZoneActionMenuPresenter>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<ZoneActionMenuView>().As<IZoneActionMenuView>();
            
            builder.RegisterEntryPoint<EmployeeActionButtonsPresenter>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<EmployeeActionButtonsView>().As<IEmployeeActionButtonsView>();
            
            builder.RegisterEntryPoint<EmployeeListPresenter>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<EmployeeListView>().As<IEmployeeListView>();

            builder.RegisterEntryPoint<DisplayCanvasSubscriptionManager>(Lifetime.Scoped);
        }
    }
}
