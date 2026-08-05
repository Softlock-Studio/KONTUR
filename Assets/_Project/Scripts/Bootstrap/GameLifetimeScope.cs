using Game.Audio;
using Game.House;
using Game.House.Model;
using Game.Input;
using Game.Localization;
using Game.Save;
using Loader.SceneController;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [Header("Loader")]
        [SerializeField] private LevelLoaderConfig _levelLoaderConfig;
        [SerializeField] private bool _isDebug = false;

        [Space]
        [Header("Other System")]
        [SerializeField] private ResourceConfig resourceConfig;
        [SerializeField] private AudioConfig audioConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_levelLoaderConfig);
            //builder.RegisterInstance(resourceConfig);
			builder.RegisterInstance(audioConfig);

            // Game-wide bindings go here. Mission-specific systems (e.g. House) are
            // registered in MissionScope, not here.
            builder.RegisterEntryPoint<LocalizationService>(Lifetime.Singleton).As<ILocalizationService>();

            // Persists across missions, unlike everything registered in MissionScope.
            //builder.Register<ResourceInventory>(Lifetime.Singleton);

            builder.Register<SaveService>(Lifetime.Singleton).As<ISaveService>();

            builder.Register<SceneController>(Lifetime.Singleton).AsSelf().WithParameter("isDebug", _isDebug);

            builder.RegisterEntryPoint<AudioService>(Lifetime.Singleton).As<IAudioService>();

            builder.RegisterEntryPoint<InputService>(Lifetime.Singleton).As<IInputService>();
        }
    }
}
