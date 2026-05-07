using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor utility for Phase 2 — wires thief prefabs into ThiefSpawner.
/// Implements AIG-81.2: Thief spawner integration.
///
/// Run: Tools → Camel Runner → Phase 2 Wire Thief (DesertBandit)
///
/// Adds a new ThiefPrefabEntry to ThiefSpawner.thiefPrefabs:
///   ThiefType: DesertBandit
///   Prefab: Assets/Models/Egypt/Characters/DesertBandit/Prefabs/DesertBandit.prefab
/// </summary>
public static class Phase2AddThief
{
    private const string DesertBanditPrefabPath = "Assets/Models/Egypt/Characters/DesertBandit/Prefabs/DesertBandit.prefab";

    [MenuItem("Tools/Camel Runner/Phase 2 Wire Thief (DesertBandit)")]
    public static void WireDesertBanditToSpawner()
    {
        Debug.Log("[Phase2AddThief] Wiring DesertBandit prefab into ThiefSpawner...");

        // Find ThiefSpawner in scene
        var spawner = Object.FindAnyObjectByType<ThiefSpawner>();
        if (spawner == null)
        {
            Debug.LogError("[Phase2AddThief] ✗ ThiefSpawner not found in scene. " +
                           "Add a ThiefSpawner component to a GameObject and run again.");
            return;
        }

        // Load the DesertBandit prefab
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(DesertBanditPrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[Phase2AddThief] ✗ DesertBandit prefab not found at {DesertBanditPrefabPath}");
            return;
        }

        // Wire it in via reflection (since thiefPrefabs is private)
        var fieldInfo = typeof(ThiefSpawner).GetField(
            "thiefPrefabs",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );

        if (fieldInfo == null)
        {
            Debug.LogError("[Phase2AddThief] ✗ Could not find thiefPrefabs field on ThiefSpawner");
            return;
        }

        var list = fieldInfo.GetValue(spawner) as System.Collections.Generic.List<ThiefSpawner.ThiefPrefabEntry>;
        if (list == null)
        {
            Debug.LogError("[Phase2AddThief] ✗ thiefPrefabs is null or not a list");
            return;
        }

        // Check if DesertBandit already wired
        foreach (var entry in list)
        {
            if (entry.thiefType == ThiefSystem.ThiefType.DesertBandit)
            {
                if (entry.prefab == prefab)
                {
                    Debug.Log("[Phase2AddThief] ✓ DesertBandit already wired to ThiefSpawner");
                    return;
                }
                else
                {
                    Debug.LogWarning("[Phase2AddThief] ⚠ DesertBandit already wired to a different prefab. Replacing...");
                    entry.prefab = prefab;
                    EditorUtility.SetDirty(spawner);
                    AssetDatabase.SaveAssets();
                    Debug.Log("[Phase2AddThief] ✓ DesertBandit prefab updated in ThiefSpawner");
                    return;
                }
            }
        }

        // Add new entry
        var newEntry = new ThiefSpawner.ThiefPrefabEntry
        {
            thiefType = ThiefSystem.ThiefType.DesertBandit,
            prefab = prefab
        };
        list.Add(newEntry);

        EditorUtility.SetDirty(spawner);
        AssetDatabase.SaveAssets();

        Debug.Log("[Phase2AddThief] ✓ DesertBandit wired to ThiefSpawner");
        Debug.Log($"[Phase2AddThief] Thief prefabs now has {list.Count} entries");

        // Validate
        spawner.ValidatePrefabAssignments();
    }
}
