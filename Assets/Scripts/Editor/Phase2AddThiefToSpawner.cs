using UnityEditor;
using UnityEngine;

/// <summary>
/// Simple editor utility to add thief prefabs to ThiefSpawner as they are delivered.
///
/// Usage:
///   1. Drag the thief prefab (e.g., DesertBandit.prefab) onto the thiefPrefab field
///   2. Select the ThiefType from dropdown
///   3. Click "Add Thief to Spawner"
///   4. Save scene
///
/// This replaces manual inspector editing and reduces null ref errors.
/// </summary>
public class Phase2AddThiefToSpawner : EditorWindow
{
    private GameObject thiefPrefab;
    private ThiefSystem.ThiefType selectedThiefType = ThiefSystem.ThiefType.DesertBandit;

    [MenuItem("Tools/Camel Runner/Phase 2 - Add Thief to Spawner")]
    public static void ShowWindow()
    {
        GetWindow<Phase2AddThiefToSpawner>("Add Thief");
    }

    private void OnGUI()
    {
        GUILayout.Label("Phase 2: Add Thief Prefab to Spawner", EditorStyles.boldLabel);

        thiefPrefab = EditorGUILayout.ObjectField("Thief Prefab", thiefPrefab, typeof(GameObject), false) as GameObject;
        selectedThiefType = (ThiefSystem.ThiefType)EditorGUILayout.EnumPopup("Thief Type", selectedThiefType);

        GUILayout.Space(10);

        if (GUILayout.Button("Add Thief to Spawner", GUILayout.Height(30)))
        {
            AddThiefToSpawner();
        }

        GUILayout.Space(10);
        GUILayout.Label("Expected Thief Types for Phase 2:", EditorStyles.label);
        GUILayout.Label("- DesertBandit (AIG-197)", EditorStyles.helpBox);
        GUILayout.Label("- Pirate (AIG-198)", EditorStyles.helpBox);
        GUILayout.Label("- ShadowThief (AIG-199)", EditorStyles.helpBox);
    }

    private void AddThiefToSpawner()
    {
        if (thiefPrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a thief prefab.", "OK");
            return;
        }

        ThiefSpawner spawner = Object.FindObjectOfType<ThiefSpawner>();
        if (spawner == null)
        {
            EditorUtility.DisplayDialog("Error", "ThiefSpawner not found in scene.", "OK");
            return;
        }

        // Use reflection to access the private thiefPrefabs list
        var field = typeof(ThiefSpawner).GetField("thiefPrefabs",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find thiefPrefabs field on ThiefSpawner.", "OK");
            return;
        }

        var prefabList = field.GetValue(spawner) as System.Collections.Generic.List<ThiefSpawner.ThiefPrefabEntry>;
        if (prefabList == null)
        {
            prefabList = new System.Collections.Generic.List<ThiefSpawner.ThiefPrefabEntry>();
            field.SetValue(spawner, prefabList);
        }

        // Check if thief type already exists in list
        var existing = prefabList.Find(e => e.thiefType == selectedThiefType);
        if (existing != null && existing.prefab != null)
        {
            EditorUtility.DisplayDialog("Already Added",
                $"{selectedThiefType} is already wired to a prefab.\n\nTo replace it, remove the old entry first.", "OK");
            return;
        }

        // Create new entry
        var entry = new ThiefSpawner.ThiefPrefabEntry
        {
            thiefType = selectedThiefType,
            prefab = thiefPrefab
        };

        // Remove old entry if it exists (with null prefab)
        prefabList.RemoveAll(e => e.thiefType == selectedThiefType);
        prefabList.Add(entry);

        EditorUtility.SetDirty(spawner);

        EditorUtility.DisplayDialog("Success",
            $"{selectedThiefType} prefab added to ThiefSpawner.\n\nRemember to save the scene!",
            "OK");

        Debug.Log($"[Phase2AddThief] ✓ {selectedThiefType} added to ThiefSpawner");
    }
}
