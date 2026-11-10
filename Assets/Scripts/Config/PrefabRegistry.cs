using System.Collections.Generic;
using UnityEngine;

public class PrefabRegistry
{
    private readonly Dictionary<string, GameObject> prefabs = new();
    private readonly List<string> errors = new();

    public IReadOnlyDictionary<string, GameObject> Prefabs => prefabs;
    public IReadOnlyList<string> Errors => errors;

    public static PrefabRegistry Build()
    {
        PrefabRegistry registry = new();

        foreach (GameObject prefab in Resources.LoadAll<GameObject>("UnitPrefabs"))
            registry.Register(prefab);

        return registry;
    }

    private void Register(GameObject prefab)
    {
        string id = ConfigIdUtility.ToKebabCase(prefab.name);

        if (prefabs.ContainsKey(id))
        {
            errors.Add($"Duplicate prefab ID '{id}'.");
            return;
        }

        prefabs.Add(id, prefab);
    }

    public bool TryGet(string id, out GameObject prefab)
    {
        return prefabs.TryGetValue(id, out prefab);
    }
}