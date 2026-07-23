using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Loader.SceneController.Editor
{
    // Renders a [SceneDropdown] string field as a popup of every .unity scene under
    // Assets/_Project (instead of a free-typed name), and flags — with a one-click fix —
    // when the selected scene isn't registered (or is disabled) in Build Settings, since
    // SceneManager.LoadSceneAsync silently fails for scenes that aren't.
    [CustomPropertyDrawer(typeof(SceneDropdownAttribute))]
    public sealed class SceneDropdownDrawer : PropertyDrawer
    {
        private const string NoneOption = "(none)";
        private const float Spacing = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (property.propertyType == SerializedPropertyType.String && NeedsBuildSettingsWarning(property.stringValue, out _))
                height += Spacing + EditorGUIUtility.singleLineHeight * 2f;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "[SceneDropdown] requires a string field.");
                return;
            }

            var popupRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            DrawScenePopup(popupRect, property, label);

            if (NeedsBuildSettingsWarning(property.stringValue, out string scenePath))
            {
                var warningRect = new Rect(
                    position.x,
                    popupRect.yMax + Spacing,
                    position.width,
                    EditorGUIUtility.singleLineHeight * 2f);
                DrawBuildSettingsWarning(warningRect, property.stringValue, scenePath);
            }
        }

        private static void DrawScenePopup(Rect rect, SerializedProperty property, GUIContent label)
        {
            List<string> sceneNames = FindProjectSceneNames();
            string current = property.stringValue;
            bool isSet = !string.IsNullOrEmpty(current);
            bool isMissing = isSet && !sceneNames.Contains(current);

            var options = new List<string> { NoneOption };
            if (isMissing) options.Add($"{current} (missing)");
            options.AddRange(sceneNames);

            int selectedIndex = !isSet ? 0 : isMissing ? 1 : options.IndexOf(current);

            int chosen = EditorGUI.Popup(rect, label.text, selectedIndex, options.ToArray());
            if (chosen == selectedIndex) return;

            property.stringValue = chosen == 0 ? string.Empty : options[chosen];
        }

        private static void DrawBuildSettingsWarning(Rect rect, string sceneName, string scenePath)
        {
            var boxRect = new Rect(rect.x, rect.y, rect.width - 90f, rect.height);
            var buttonRect = new Rect(boxRect.xMax + 4f, rect.y, 86f, rect.height);

            EditorGUI.HelpBox(boxRect, $"'{sceneName}' is not enabled in Build Settings — it will fail to load at runtime.", MessageType.Warning);

            if (string.IsNullOrEmpty(scenePath)) return;

            if (GUI.Button(buttonRect, "Add"))
                AddToBuildSettings(scenePath);
        }

        // True when sceneName is set but either missing from the project entirely, or present
        // in the project but not (enabled) in EditorBuildSettings.scenes.
        private static bool NeedsBuildSettingsWarning(string sceneName, out string scenePath)
        {
            scenePath = null;
            if (string.IsNullOrEmpty(sceneName)) return false;

            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (Path.GetFileNameWithoutExtension(scene.path) == sceneName)
                    return !scene.enabled;
            }

            scenePath = FindProjectScenePath(sceneName);
            return true;
        }

        private static void AddToBuildSettings(string scenePath)
        {
            var scenes = EditorBuildSettings.scenes.ToList();

            int existing = scenes.FindIndex(s => s.path == scenePath);
            if (existing >= 0)
                scenes[existing].enabled = true;
            else
                scenes.Add(new EditorBuildSettingsScene(scenePath, true));

            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private static List<string> FindProjectSceneNames()
        {
            return AssetDatabase.FindAssets("t:Scene", new[] { "Assets/_Project" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(name => name)
                .ToList();
        }

        private static string FindProjectScenePath(string sceneName)
        {
            return AssetDatabase.FindAssets("t:Scene", new[] { "Assets/_Project" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault(path => Path.GetFileNameWithoutExtension(path) == sceneName);
        }
    }
}
