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
        }
    }
}
