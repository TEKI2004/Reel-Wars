using UnityEditor;
using UnityEngine;

public static class UnitPresentationSetupEditor
{
    [MenuItem("Tools/Reel Wars/Auto-Wire Unit Presentations")]
    private static void AutoWireUnitPresentations()
    {
        FloatingUnitTag unitTagPrefab = FindUnitTagPrefab();

        if (unitTagPrefab == null)
        {
            Debug.LogError("FloatingUnitTag prefab could not be found.");
            return;
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[]
        {
            "Assets/Resources/UnitPrefabs",
            "Assets/Prefabs"
        });

        int updatedCount = 0;

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = PrefabUtility.LoadPrefabContents(path);

            try
            {
                if (root.GetComponent<UnitBehaviour>() == null)
                    continue;

                UnitPresentation presentation = root.GetComponent<UnitPresentation>();

                if (presentation == null)
                    presentation = root.AddComponent<UnitPresentation>();

                presentation.EditorAutoWire(unitTagPrefab);
                PrefabUtility.SaveAsPrefabAsset(root, path);

                updatedCount++;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Auto-wired {updatedCount} unit prefabs.");
    }

    private static FloatingUnitTag FindUnitTagPrefab()
    {
        string[] guids = AssetDatabase.FindAssets("FloatingUnitTag t:Prefab");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            FloatingUnitTag unitTag = prefab.GetComponentInChildren<FloatingUnitTag>(true);

            if (unitTag != null)
                return unitTag;
        }

        return null;
    }
}