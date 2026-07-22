using CameraSystem;
using Game.Audio;
using Game.House;
using Game.House.Model;
using Game.House.Presentation;
using VContainer;
using VContainer.Unity;

namespace Game.Mission
{
    public sealed class MissionScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<ZoneRegistry>();

            builder.Register<HouseModel>(Lifetime.Scoped);

            builder.RegisterEntryPoint<HousePresenter>(Lifetime.Scoped);

            // TEMPORARY swap for the real Canvas view once available.
            builder.RegisterComponentInHierarchy<DebugHouseConsoleView>().As<IHouseView>();

            // Also exposed as ICameraObservationService — AudioEmitter (scene-level, injected via
            // this same scope) uses it to decide whether a world sound is currently audible to the
            // player (only through the selected camera's room), not registered in GameLifetimeScope
            // since it depends on ZoneRegistry, which is mission-scoped.
            builder.RegisterComponentInHierarchy<CamerasView>().As<ICamerasView>();
            builder.Register<CamerasModel>(Lifetime.Scoped).AsSelf().As<ICameraObservationService>();
            builder.RegisterEntryPoint<CamerasPresenter>(Lifetime.Scoped);
        }
    }
}
