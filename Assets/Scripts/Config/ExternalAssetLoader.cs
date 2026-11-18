using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ExternalAssetLoader
{
    private const float UnitRootGroundOffset = -0.9f;

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

    public static async Task<GameObject> CreateUnitPrefabAsync(
        UnitBehaviour unitTemplate,
        string modelPath,
        string unitId,
        UnitModelConfigData modelSettings
    )
    {
        UnitBehaviour runtimeUnit = Object.Instantiate(unitTemplate, GetRuntimePrefabsRoot());

        runtimeUnit.gameObject.name = $"{unitId} (Runtime UnitPrefab)";

        runtimeUnit.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        runtimeUnit.transform.localScale = Vector3.one;

        runtimeUnit.gameObject.SetActive(true);

        Transform modelRoot = new GameObject("ModelRoot").transform;
        modelRoot.SetParent(runtimeUnit.VisualRoot, false);

        GltfImport gltfImport = new();

        try
        {
            byte[] modelBytes = File.ReadAllBytes(modelPath);

            Uri modelUri = new(Path.GetFullPath(modelPath), UriKind.Absolute);

            bool loaded = await gltfImport.Load(modelBytes, modelUri);

            if (!loaded)
            {
                throw new InvalidDataException($"Failed to load GLB model: {modelPath}");
            }

            bool instantiated = await gltfImport.InstantiateMainSceneAsync(modelRoot);

            if (!instantiated)
            {
                throw new InvalidDataException($"Failed to instantiate GLB model: {modelPath}");
            }

            float groundY = runtimeUnit.transform.position.y + UnitRootGroundOffset;
            Bounds modelBounds = NormalizeModel(modelRoot, groundY, modelSettings);

            runtimeUnit.CalibrateVisualAnchors(modelBounds);

            loadedImports.Add(gltfImport);

            return runtimeUnit.gameObject;
        }
        catch
        {
            gltfImport.Dispose();
            Object.Destroy(runtimeUnit.gameObject);
            throw;
        }
    }

    private static Bounds NormalizeModel(Transform modelRoot, float groundY, UnitModelConfigData settings)
    {
        modelRoot.localPosition = Vector3.zero;
        modelRoot.localRotation = Quaternion.Euler(0f, settings.RotationY ?? 0f, 0f);

        modelRoot.localScale = Vector3.one;

        Bounds bounds = CalculateBounds(modelRoot);

        if (bounds.size.y <= Mathf.Epsilon)
        {
            throw new InvalidOperationException($"Model '{modelRoot.name}' has no measurable height.");
        }

        float targetHeight = settings.Height.Value;
        float scale = targetHeight / bounds.size.y;
        modelRoot.localScale = Vector3.one * scale;

        bounds = CalculateBounds(modelRoot);

        float targetGroundY = groundY + (settings.GroundOffset ?? 0f);
        float verticalCorrection = targetGroundY - bounds.min.y;
        modelRoot.position += Vector3.up * verticalCorrection;

        return CalculateBounds(modelRoot);
    }

    private static Bounds CalculateBounds(Transform root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);

        if (renderers.Length == 0)
        {
            throw new InvalidOperationException($"No Renderer was found under '{root.name}'.");
        }

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        return bounds;
    }

    private static Transform GetRuntimePrefabsRoot()
    {
        if (runtimePrefabsRoot != null)
            return runtimePrefabsRoot;

        GameObject root = new("Runtime UnitPrefabs");

        root.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        root.transform.localScale = Vector3.one;
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