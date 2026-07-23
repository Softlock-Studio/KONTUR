using System;
using Game.House;
using UnityEngine;

namespace Game.UI.House
{
    // Icon + localization key per ResourceType, used by ResourceGridPresenter. One shared
    // instance — resource types/icons/names are the same across every scene, unlike MapUiConfig.
    [CreateAssetMenu(menuName = "KONTUR/UI/Resource Catalog Config", fileName = "ResourceCatalogConfig")]
    public sealed class ResourceCatalogConfig : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public ResourceType Type;
            public Sprite Icon;
            public string NameLocalizationKey;
        }

        public Entry[] Resources;

        public bool TryFind(ResourceType type, out Entry entry)
        {
            foreach (Entry candidate in Resources)
            {
                if (candidate.Type != type) continue;
                entry = candidate;
                return true;
            }

            entry = default;
            return false;
        }
    }
}
