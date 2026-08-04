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
            [Tooltip("Тип ресурса (йод, лампочка и т.д.).")]
            public ResourceType Type;
            [Tooltip("Иконка ресурса, отображаемая в UI.")]
            public Sprite Icon;
            [Tooltip("Ключ локализации для отображаемого названия ресурса.")]
            public string NameLocalizationKey;
        }

        [Tooltip("Список всех ресурсов и их отображения (иконка + название) для UI.")]
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
