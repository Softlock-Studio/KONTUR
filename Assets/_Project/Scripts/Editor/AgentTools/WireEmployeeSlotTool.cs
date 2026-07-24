using Game.UI.House;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.EditorTools
{
    // Adds EmployeeSlotView to Employee Slot.prefab and wires it to the existing "Text (TMP)"
    // (Unavailable placeholder) and nested "Employee Card" (Name / Goal Text) children.
    // Idempotent: safe to run more than once.
    public static class WireEmployeeSlotTool
    {
        private const string PrefabPath = "Assets/_Project/Prefabs/UI/Employee Slot.prefab";

        [MenuItem("Tools/AgentTools/Wire Employee Slot")]
        public static void Run()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(PrefabPath);
            try
            {
                var view = root.GetComponent<EmployeeSlotView>();
                if (view == null) view = root.AddComponent<EmployeeSlotView>();

                Transform unavailable = root.transform.Find("Text (TMP)");
                Transform employeeCard = root.transform.Find("Employee Card");

                if (unavailable == null || employeeCard == null)
                {
                    Debug.LogError("[WireEmployeeSlotTool] Expected children 'Text (TMP)' and 'Employee Card' " +
                                   "not found under Employee Slot — prefab structure may have changed, aborting.");
                    return;
                }

                TMP_Text nameLabel = employeeCard.Find("Name")?.GetComponent<TMP_Text>();
                Image portrait = employeeCard.Find("Image")?.GetComponent<Image>();
                TMP_Text goalText = employeeCard.Find("Goal Group/Goal Text")?.GetComponent<TMP_Text>();
                TMP_Text destinationText = employeeCard.Find("Destination Group/Destination Text")?.GetComponent<TMP_Text>();

                var serialized = new SerializedObject(view);
                serialized.FindProperty("unavailableRoot").objectReferenceValue = unavailable.gameObject;
                serialized.FindProperty("employeeCardRoot").objectReferenceValue = employeeCard.gameObject;
                serialized.FindProperty("nameLabel").objectReferenceValue = nameLabel;
                serialized.FindProperty("portrait").objectReferenceValue = portrait;
                serialized.FindProperty("goalText").objectReferenceValue = goalText;
                serialized.FindProperty("destinationText").objectReferenceValue = destinationText;
                serialized.ApplyModifiedPropertiesWithoutUndo();    

                PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
                Debug.Log("[WireEmployeeSlotTool] Wired EmployeeSlotView on Employee Slot.prefab.");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }
}
