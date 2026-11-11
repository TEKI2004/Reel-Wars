using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ExternalAssetLoader
{
    private static readonly List<GltfImport> loadedImports = new();

    private static Transform runtimePrefabsRoot;

    public static Sprite LoadSprite(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);

        Texture2D texture = new(2, 2, TextureFormat.RGBA32, false);

        if (!texture.LoadImage(bytes, true))
        {
            Object.Destroy(texture);
            throw new InvalidDataException($"Failed to load image: {path}");
        }

        texture.name = Path.GetFileNameWithoutExtension(path);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect
        );

        sprite.name = texture.name;

        return sprite;
    }

    public static async Task<GameObject> CreateUnitPrefabAsync(UnitBehaviour unitTemplate, string modelPath, string unitId)
    {
        Transform templatesRoot = GetRuntimePrefabsRoot();

        UnitBehaviour runtimeUnit = Object.Instantiate(unitTemplate, templatesRoot);
        runtimeUnit.gameObject.name = $"{unitId} (Runtime UnitPrefab)";
        runtimeUnit.gameObject.SetActive(true);

        GltfImport gltfImport = new();

        try
        {
            byte[] modelBytes = File.ReadAllBytes(modelPath);
            Uri modelUri = new(Path.GetFullPath(modelPath));

            bool loaded = await gltfImport.Load(modelBytes, modelUri);

            if (!loaded)
                throw new InvalidDataException($"Failed to load GLB model: {modelPath}");

            bool instantiated = await gltfImport.InstantiateMainSceneAsync(runtimeUnit.VisualRoot);

            if (!instantiated)
                throw new InvalidDataException($"Failed to instantiate GLB model: {modelPath}");

            loadedImports.Add(gltfImport);

            return runtimeUnit.gameObject;
        }
        catch (Exception exception)
        {
            Debug.LogError($"Error loading unit prefab for '{unitId}' from '{modelPath}': {exception.Message}");
            gltfImport.Dispose();
            Object.Destroy(runtimeUnit.gameObject);
            throw;
        }
    }

    private static Transform GetRuntimePrefabsRoot()
    {
        if (runtimePrefabsRoot != null)
            return runtimePrefabsRoot;

        GameObject root = new("Runtime UnitPrefabs");

        root.SetActive(false);
        Object.DontDestroyOnLoad(root);

        runtimePrefabsRoot = root.transform;

        return runtimePrefabsRoot;
    }

    public static void ReleaseAll()
    {
        foreach (GltfImport gltfImport in loadedImports)
            gltfImport.Dispose();

        loadedImports.Clear();

        if (runtimePrefabsRoot != null)
        {
            Object.Destroy(runtimePrefabsRoot.gameObject);
            runtimePrefabsRoot = null;
        }
    }
}