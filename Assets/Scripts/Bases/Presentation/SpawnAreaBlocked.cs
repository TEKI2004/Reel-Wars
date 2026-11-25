using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpawnAreaBlocked : MonoBehaviour
{
    [BoxGroup("Appearance")]
    [SerializeField, Required] private MeshRenderer targetRenderer;

    [BoxGroup("Appearance")]
    [HorizontalGroup("Appearance/Visuals")]
    [LabelWidth(70)]
    [SerializeField] private Color flashColor = Color.red;

    [BoxGroup("Appearance")]
    [HorizontalGroup("Appearance/Visuals")]
    [LabelWidth(85)]
    [SerializeField] private float minIntensity = -3f;

    [BoxGroup("Appearance")]
    [HorizontalGroup("Appearance/Visuals")]
    [LabelWidth(85)]
    [SerializeField] private float maxIntensity = 3f;

    [BoxGroup("Appearance")]
    [Button("Flash")]
    private void TestFlash()
    {
        Flash();
    }

    [BoxGroup("Timing")]
    [LabelWidth(70)]
    [SerializeField, MinValue(0.05f), SuffixLabel("s", Overlay = true)] private float duration = 0.35f;

    private Material material;
    private Coroutine flashCoroutine;

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        material = targetRenderer.material;
        material.EnableKeyword("_EMISSION");

        SetEmissionOff();
    }

    public void Flash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float elapsed = 0f;

        SetEmission(maxIntensity);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float intensity = Mathf.Lerp(maxIntensity, minIntensity, t);

            SetEmission(intensity);

            yield return null;
        }

        SetEmissionOff();
        flashCoroutine = null;
    }

    private void SetEmission(float intensity)
    {
        Color emission = flashColor * Mathf.Pow(2f, intensity);
        material.SetColor(EmissionColor, emission);
    }

    private void SetEmissionOff()
    {
        material.SetColor(EmissionColor, Color.black);
    }

    private void OnDestroy()
    {
        if (material != null)
            Destroy(material);
    }
}