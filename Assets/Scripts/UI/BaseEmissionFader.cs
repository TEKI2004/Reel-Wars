using UnityEngine;

public class BaseEmissionFader : MonoBehaviour
{
    [SerializeField] private MeshRenderer targetRenderer;
    [SerializeField] private Color baseColor = Color.yellow;

    [SerializeField] private float maxIntensity = 2.4f;
    [SerializeField] private float minIntensity = -5f;

    private Material material;

    private void Awake()
    {
        material = targetRenderer.material;
        material.EnableKeyword("_EMISSION");
    }

    public void SetLitPercent(float percent)
    {
        percent = Mathf.Clamp01(percent);

        float intensity = Mathf.Lerp(minIntensity, maxIntensity, percent);

        material.SetColor("_EmissionColor", baseColor * Mathf.Pow(2f, intensity));
    }
}