using Game.Audio;
using Game.House;
using Game.House.Model;
using Game.Localization;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private ResourceConfig resourceConfig;
        [SerializeField] private AudioConfig audioConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            // Game-wide bindings go here. Mission-specific systems (e.g. House) are
            // registered in MissionScope, not here.
            builder.RegisterEntryPoint<LocalizationService>(Lifetime.Singleton).As<ILocalizationService>();

            // Persists across missions, unlike everything registered in MissionScope.
            builder.RegisterInstance(resourceConfig);
            builder.Register<ResourceInventory>(Lifetime.Singleton);

            builder.RegisterInstance(audioConfig);
            builder.RegisterEntryPoint<AudioService>(Lifetime.Singleton).As<IAudioService>();
        }
    }
}
