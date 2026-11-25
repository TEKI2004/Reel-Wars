using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class OscarSeasonView : MonoBehaviour
{
    [Title("References")]
    [Required][SerializeField] private Transform oscar;
    [Required][SerializeField] private Transform homeAnchor;
    [SerializeField] private Transform exitAnchor;
    [Required][SerializeField] private Transform laneAnchor;

    [Title("Popularity Fill")]
    [Required][SerializeField] private MeshRenderer popularityFillRenderer;
    [Required][SerializeField] private Material goldMaterial;
    [Required][SerializeField] private Material emissiveMaterial;

    [Title("Timing")]
    [MinValue(0.01f)][SerializeField] private float exitDuration = 0.2f;
    [MinValue(0.01f)][SerializeField] private float laneDuration = 0.4f;
    [MinValue(0.01f)][SerializeField] private float outroDuration = 0.4f;

    [Title("Bounce")]
    [MinValue(1f)][SerializeField] private float bounceScale = 1.12f;
    [MinValue(0.01f)][SerializeField] private float bounceDuration = 0.12f;

    private Vector3 baseScale;

    private void Awake()
    {
        baseScale = oscar.localScale;
    }

    public IEnumerator PlayIntro()
    {
        popularityFillRenderer.sharedMaterial = goldMaterial;

        if (exitAnchor != null) yield return MoveTo(exitAnchor.position, exitDuration);

        yield return MoveTo(laneAnchor.position, laneDuration);
        yield return ScaleTo(baseScale * bounceScale, bounceDuration);
        yield return ScaleTo(baseScale, bounceDuration);
    }

    public IEnumerator PlayOutro()
    {
        if (exitAnchor != null)
        {
            yield return MoveTo(exitAnchor.position, outroDuration * 0.5f);
            yield return MoveTo(homeAnchor.position, outroDuration * 0.5f);
        }
        else
        {
            yield return MoveTo(homeAnchor.position, outroDuration);
        }

        popularityFillRenderer.sharedMaterial = emissiveMaterial;
    }

    [Button(ButtonSizes.Large)]
    [GUIColor(0.3f, 0.8f, 0.3f)]
    private void TestIntro()
    {
        if (!Application.isPlaying) return;

        StopAllCoroutines();
        StartCoroutine(PlayIntro());
    }

    [Button(ButtonSizes.Large)]
    [GUIColor(0.9f, 0.5f, 0.2f)]
    private void TestOutro()
    {
        if (!Application.isPlaying) return;

        StopAllCoroutines();
        StartCoroutine(PlayOutro());
    }

    [HorizontalGroup("Snap")]
    [Button]
    private void SnapHome()
    {
        oscar.position = homeAnchor.position;
    }

    [HorizontalGroup("Snap")]
    [Button]
    private void SnapExit()
    {
        if (exitAnchor != null) oscar.position = exitAnchor.position;
    }

    [HorizontalGroup("Snap")]
    [Button]
    private void SnapLane()
    {
        oscar.position = laneAnchor.position;
    }

    private IEnumerator MoveTo(Vector3 target, float duration)
    {
        Vector3 start = oscar.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));

            oscar.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        oscar.position = target;
    }

    private IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 start = oscar.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));

            oscar.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }

        oscar.localScale = target;
    }
}