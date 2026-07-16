using Assets.SimpleLocalization.Scripts.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Localization.Editor
{
    [CustomEditor(typeof(LocalizedTextTMP))]
    public sealed class LocalizedTextTMPEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Localization Editor"))
            {
                LocalizationEditorWindow.Open();
            }
        }
    }
}
