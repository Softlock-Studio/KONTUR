using Game.Localization;
using VContainer;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Game-wide bindings go here. Mission-specific systems (e.g. House) are
            // registered in MissionScope, not here.
            builder.RegisterEntryPoint<LocalizationService>(Lifetime.Singleton).As<ILocalizationService>();
        }
    }
}
