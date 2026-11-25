using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class OscarSeasonTimerView : MonoBehaviour
{
    [Title("Timer")]
    [Required][SerializeField] private Transform timerRoot;
    [Required][SerializeField] private Transform cylinder;
    [Required][SerializeField] private Transform leftSphere;
    [Required][SerializeField] private Transform rightSphere;

    [Title("Moving Spotlights")]
    [Required][SerializeField] private Transform leftSpotlight;
    [Required][SerializeField] private Transform rightSpotlight;
    [Required][SerializeField] private MeshRenderer leftSpotlightRenderer;
    [Required][SerializeField] private MeshRenderer rightSpotlightRenderer;

    [Title("Light Cones")]
    [Required][SerializeField] private GameObject leftLightCone;
    [Required][SerializeField] private GameObject rightLightCone;

    [Title("Static Spotlights")]
    [Required][SerializeField] private GameObject leftStaticSpotlight;
    [Required][SerializeField] private GameObject rightStaticSpotlight;

    [Title("Materials")]
    [Required][SerializeField] private Material emissiveMaterial;
    [Required][SerializeField] private Material glassMaterial;

    [Title("Timer Settings")]
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 5f;
    [SerializeField] private float spotlightOffset = 0.2f;

    [Title("Intro / Outro")]
    [SerializeField] private float introDuration = 0.3f;
    [SerializeField] private float outroBounceScale = 1.1f;
    [SerializeField] private float outroBounceDuration = 0.1f;
    [SerializeField] private float outroShrinkDuration = 0.2f;

    [Title("Flicker")]
    [SerializeField] private float flickerStartTime = 3f;
    [SerializeField] private Vector2 flickerOffDuration = new(0.04f, 0.09f);
    [SerializeField] private Vector2 flickerOnDuration = new(0.08f, 0.25f);

    [Title("Testing")]
    [SerializeField] private float testDuration = 10f;

    private Coroutine flickerRoutine;
    private Coroutine testRoutine;

    public IEnumerator PlayIntro()
    {
        StopFlicker();

        timerRoot.gameObject.SetActive(true);
        leftSpotlight.gameObject.SetActive(true);
        rightSpotlight.gameObject.SetActive(true);

        SetOverallScale(Vector3.one);
        SetSpotlightsLit(false);
        SetExtent(0f);

        float elapsed = 0f;

        while (elapsed < introDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / introDuration));

            SetExtent(Mathf.Lerp(0f, maxScale, t));
            yield return null;
        }

        SetExtent(maxScale);
        SetSpotlightsLit(true);
    }

    public IEnumerator PlayTimer(float duration)
    {
        float remaining = duration;
        bool flickerStarted = false;

        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;

            if (!flickerStarted && remaining <= flickerStartTime)
            {
                flickerStarted = true;
                flickerRoutine = StartCoroutine(FlickerRoutine());
            }

            SetProgress(remaining / duration);
            yield return null;
        }

        SetExtent(minScale);
        StopFlicker();
        SetSpotlightsLit(false);
    }

    public IEnumerator PlayOutro()
    {
        StopFlicker();
        SetSpotlightsLit(false);

        yield return ScaleAll(Vector3.one * outroBounceScale, outroBounceDuration);
        yield return ScaleAll(Vector3.zero, outroShrinkDuration);

        timerRoot.gameObject.SetActive(false);
        leftSpotlight.gameObject.SetActive(false);
        rightSpotlight.gameObject.SetActive(false);

        SetOverallScale(Vector3.one);
    }

    public void SetProgress(float progress)
    {
        SetExtent(Mathf.Lerp(minScale, maxScale, Mathf.Clamp01(progress)));
    }

    private void SetExtent(float extent)
    {
        Vector3 cylinderScale = cylinder.localScale;
        cylinderScale.y = extent;
        cylinder.localScale = cylinderScale;

        SetLocalX(leftSphere, -extent);
        SetLocalX(rightSphere, extent);

        float spotlightX = Mathf.Max(0f, extent - spotlightOffset);

        SetLocalX(leftSpotlight, -spotlightX);
        SetLocalX(rightSpotlight, spotlightX);
    }

    private void SetSpotlightsLit(bool lit)
    {
        leftSpotlightRenderer.sharedMaterial = lit ? emissiveMaterial : glassMaterial;
        rightSpotlightRenderer.sharedMaterial = lit ? emissiveMaterial : glassMaterial;

        leftLightCone.SetActive(lit);
        rightLightCone.SetActive(lit);

        leftStaticSpotlight.SetActive(lit);
        rightStaticSpotlight.SetActive(lit);
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            SetSpotlightsLit(false);
            yield return new WaitForSeconds(Random.Range(flickerOffDuration.x, flickerOffDuration.y));

            SetSpotlightsLit(true);
            yield return new WaitForSeconds(Random.Range(flickerOnDuration.x, flickerOnDuration.y));
        }
    }

    private void StopFlicker()
    {
        if (flickerRoutine == null) return;

        StopCoroutine(flickerRoutine);
        flickerRoutine = null;
    }

    private IEnumerator ScaleAll(Vector3 targetScale, float duration)
    {
        Vector3 timerStart = timerRoot.localScale;
        Vector3 leftStart = leftSpotlight.localScale;
        Vector3 rightStart = rightSpotlight.localScale;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));

            timerRoot.localScale = Vector3.Lerp(timerStart, targetScale, t);
            leftSpotlight.localScale = Vector3.Lerp(leftStart, targetScale, t);
            rightSpotlight.localScale = Vector3.Lerp(rightStart, targetScale, t);

            yield return null;
        }

        SetOverallScale(targetScale);
    }

    private void SetOverallScale(Vector3 scale)
    {
        timerRoot.localScale = scale;
        leftSpotlight.localScale = scale;
        rightSpotlight.localScale = scale;
    }

    private static void SetLocalX(Transform target, float x)
    {
        Vector3 position = target.localPosition;
        position.x = x;
        target.localPosition = position;
    }

    [Button(ButtonSizes.Large)]
    [GUIColor(0.3f, 0.8f, 0.3f)]
    private void TestFullSequence()
    {
        if (!Application.isPlaying) return;

        if (testRoutine != null) StopCoroutine(testRoutine);
        StopFlicker();

        testRoutine = StartCoroutine(TestSequence());
    }

    [HorizontalGroup("Animation")]
    [Button]
    private void TestIntro()
    {
        if (!Application.isPlaying) return;
        StartTestCoroutine(PlayIntro());
    }

    [HorizontalGroup("Animation")]
    [Button]
    private void TestOutro()
    {
        if (!Application.isPlaying) return;
        StartTestCoroutine(PlayOutro());
    }

    [HorizontalGroup("Spotlight")]
    [Button]
    private void SpotlightOn()
    {
        SetSpotlightsLit(true);
    }

    [HorizontalGroup("Spotlight")]
    [Button]
    private void SpotlightOff()
    {
        SetSpotlightsLit(false);
    }

    [HorizontalGroup("Timer")]
    [Button]
    private void Full()
    {
        timerRoot.gameObject.SetActive(true);
        leftSpotlight.gameObject.SetActive(true);
        rightSpotlight.gameObject.SetActive(true);

        SetOverallScale(Vector3.one);
        SetExtent(maxScale);
    }

    [HorizontalGroup("Timer")]
    [Button]
    private void Half()
    {
        timerRoot.gameObject.SetActive(true);
        leftSpotlight.gameObject.SetActive(true);
        rightSpotlight.gameObject.SetActive(true);

        SetOverallScale(Vector3.one);
        SetProgress(0.5f);
    }

    [HorizontalGroup("Timer")]
    [Button]
    private void Empty()
    {
        timerRoot.gameObject.SetActive(true);
        leftSpotlight.gameObject.SetActive(true);
        rightSpotlight.gameObject.SetActive(true);

        SetOverallScale(Vector3.one);
        SetExtent(minScale);
    }

    private IEnumerator TestSequence()
    {
        yield return PlayIntro();
        yield return PlayTimer(testDuration);
        yield return PlayOutro();

        testRoutine = null;
    }

    private void StartTestCoroutine(IEnumerator routine)
    {
        if (testRoutine != null) StopCoroutine(testRoutine);
        StopFlicker();

        testRoutine = StartCoroutine(TestRoutine(routine));
    }

    private IEnumerator TestRoutine(IEnumerator routine)
    {
        yield return routine;
        testRoutine = null;
    }
}