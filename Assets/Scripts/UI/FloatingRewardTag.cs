using System.Collections;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class FloatingRewardTag : MonoBehaviour
{
    [Title("References")]
    [Required][SerializeField] private TextMeshPro moneyText;
    [Required][SerializeField] private TextMeshPro popularityText;

    private const float moneyDuration = 0.6f;
    private const float moneyUpDistance = 1f;
    private const float moneyForwardDistance = 0.34f;
    private const float moneyFadeStart = 0.3f;
    private const float moneyStartOpacity = 0.75f;
    private const float moneyEndRotation = 6f;

    private const float popularityDuration = 0.6f;
    private const float popularityUpDistance = 1f;
    private const float popularityFadeStart = 0.3f;

    public void Show(RewardResult reward, Vector3 moneySpawnPosition, Vector3 popularitySpawnPosition, Vector3 moveDirection, Material popularityMaterial)
    {
        if (Camera.main != null) transform.rotation = Camera.main.transform.rotation;

        moneyText.gameObject.SetActive(reward.Money > 0);
        popularityText.gameObject.SetActive(reward.Popularity > 0);

        if (reward.Money > 0)
        {
            moneyText.text = $"+{reward.Money:N0}M $";
            moneyText.transform.position = moneySpawnPosition;
            StartCoroutine(AnimateMoney(moveDirection.normalized));
        }

        if (reward.Popularity > 0)
        {
            popularityText.text = $"+{reward.Popularity:N0}";
            popularityText.fontSharedMaterial = popularityMaterial;
            popularityText.transform.position = popularitySpawnPosition;
            StartCoroutine(AnimatePopularity());
        }

        Destroy(gameObject, Mathf.Max(moneyDuration, popularityDuration) + 0.1f);
    }

    private IEnumerator AnimateMoney(Vector3 moveDirection)
    {
        Vector3 startPosition = moneyText.transform.position;
        Vector3 endPosition = startPosition + Vector3.up * moneyUpDistance + moveDirection * moneyForwardDistance;

        Quaternion startRotation = moneyText.transform.localRotation;
        float directionSign = Mathf.Sign(moveDirection.x);
        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 0f, -moneyEndRotation * directionSign);

        Color baseColor = moneyText.color;
        baseColor.a = moneyStartOpacity;
        moneyText.color = baseColor;

        float elapsed = 0f;

        while (elapsed < moneyDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moneyDuration);
            float movementT = 1f - Mathf.Pow(1f - t, 3f);

            moneyText.transform.position = Vector3.Lerp(startPosition, endPosition, movementT);
            moneyText.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, movementT);

            float opacity = moneyStartOpacity;

            if (t >= moneyFadeStart)
            {
                float fadeT = Mathf.InverseLerp(moneyFadeStart, 1f, t);
                opacity = moneyStartOpacity * Mathf.Pow(1f - fadeT, 2f);
            }

            Color color = moneyText.color;
            color.a = opacity;
            moneyText.color = color;

            yield return null;
        }
    }

    private IEnumerator AnimatePopularity()
    {
        Vector3 startPosition = popularityText.transform.position;
        Vector3 endPosition = startPosition + Vector3.up * popularityUpDistance;

        Color baseColor = popularityText.color;
        baseColor.a = 1f;
        popularityText.color = baseColor;

        float elapsed = 0f;

        while (elapsed < popularityDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / popularityDuration);
            float movementT = 1f - Mathf.Pow(1f - t, 3f);

            popularityText.transform.position = Vector3.Lerp(startPosition, endPosition, movementT);

            float opacity = 1f;

            if (t >= popularityFadeStart)
            {
                float fadeT = Mathf.InverseLerp(popularityFadeStart, 1f, t);
                opacity = Mathf.Pow(1f - fadeT, 2f);
            }

            Color color = popularityText.color;
            color.a = opacity;
            popularityText.color = color;

            yield return null;
        }
    }
}