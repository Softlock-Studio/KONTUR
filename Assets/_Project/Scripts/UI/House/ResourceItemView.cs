using Game.House;
using Game.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.House
{
    public sealed class ResourceItemView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private LocalizedTextTMP nameLabel;
        [SerializeField] private TMP_Text countLabel;

        public ResourceType Type { get; private set; }

        public void Bind(ResourceType type, Sprite iconSprite, string nameLocalizationKey, int count)
        {
            Type = type;
            if (icon != null) icon.sprite = iconSprite;
            if (nameLabel != null) nameLabel.SetKey(nameLocalizationKey);
            SetCount(count);
        }

        public void SetCount(int count)
        {
            if (countLabel != null) countLabel.text = count.ToString();
        }
    }
}
