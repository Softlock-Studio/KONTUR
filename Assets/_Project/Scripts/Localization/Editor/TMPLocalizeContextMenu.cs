using TMPro;
using UnityEditor;

namespace Game.Localization.Editor
{
    internal static class TMPLocalizeContextMenu
    {
        [MenuItem("CONTEXT/TextMeshProUGUI/Localize")]
        private static void LocalizeUGUI(MenuCommand command) => Localize((TextMeshProUGUI) command.context);

        [MenuItem("CONTEXT/TextMeshPro/Localize")]
        private static void LocalizeWorld(MenuCommand command) => Localize((TextMeshPro) command.context);

        private static void Localize(TMP_Text component)
        {
            if (component.GetComponent<LocalizedTextTMP>() != null) return;

            Undo.AddComponent<LocalizedTextTMP>(component.gameObject);
        }
    }
}
