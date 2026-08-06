using System.Collections.Generic;
using Game.House;
using Game.Mission;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI.House
{
    public sealed class ResourceGridPresenter : MonoBehaviour
    {
        [SerializeField] private ResourceCatalogConfig catalog;
        [SerializeField] private Transform itemParent;
        [SerializeField] private ResourceItemView itemPrefab;

        private readonly Dictionary<ResourceType, ResourceItemView> items = new();

        private IObjectResolver resolver;

        // Lazy, not resolved in Start(): HousePresenter (a VContainer entry point) calls
        // view.RenderResources -> ... -> UpdateItem synchronously as part of its own Start(),
        // which can run before this MonoBehaviour's own Start() — caching the resolver up front
        // raced that and hit a NullReferenceException. The MissionScope container is guaranteed
        // to exist by the time anything calls in here (HousePresenter was itself resolved from
        // it), so resolving on first use side-steps the ordering entirely.
        private IObjectResolver Resolver => resolver ??= LifetimeScope.Find<MissionScope>(gameObject.scene).Container;

        // IObjectResolver.Instantiate rather than plain UnityEngine.Object.Instantiate, in case
        // ResourceItemView or a future nested component ever needs [Inject]/Auto Inject wiring.
        public void Render(IReadOnlyDictionary<ResourceType, int> counts)
        {
            foreach (var pair in counts)
                UpdateItem(pair.Key, pair.Value);
        }

        public void UpdateItem(ResourceType type, int count)
        {
            if (items.TryGetValue(type, out ResourceItemView item))
            {
                item.SetCount(count);
                return;
            }

            catalog.TryFind(type, out ResourceCatalogConfig.Entry entry);
            item = Resolver.Instantiate(itemPrefab, itemParent);
            item.Bind(type, entry.Icon, entry.NameLocalizationKey, count);
            items[type] = item;
        }
    }
}
