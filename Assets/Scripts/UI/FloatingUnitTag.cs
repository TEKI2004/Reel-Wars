using Sirenix.OdinInspector;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingUnitTag : MonoBehaviour
{
    [Title("References")]
    [Required][SerializeField] private TextMeshPro moneyText;
    [Required][SerializeField] private TextMeshPro popularityText;
    [Required][SerializeField] private TextMeshPro feedbackText;

    private const float moneyDuration = 0.9f;
    private const float moneyUpDistance = 1f;
    private const float moneyForwardDistance = 0.34f;
    private const float moneyFadeStart = 0.3f;
    private const float moneyStartOpacity = 0.75f;
    private const float moneyEndRotation = 6f;

    private const float popularityDuration = 0.9f;
    private const float popularityUpDistance = 1f;
    private const float popularityFadeStart = 0.3f;

    private const float feedbackDuration = 0.9f;
    private const float feedbackUpDistance = 0.8f;
    private const float feedbackFadeStart = 0.3f;

    public void ShowReward(RewardResult reward, Vector3 moneySpawnPosition, Vector3 popularitySpawnPosition, Vector3 moveDirection, Material popularityMaterial)
    {
        FaceCamera();

        moneyText.gameObject.SetActive(reward.Money > 0);
        popularityText.gameObject.SetActive(reward.Popularity > 0);
        feedbackText.gameObject.SetActive(false);

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
            StartCoroutine(AnimateVertical(popularityText, popularityDuration, popularityUpDistance, popularityFadeStart));
        }

        Destroy(gameObject, Mathf.Max(moneyDuration, popularityDuration) + 0.1f);
    }

    public void ShowFeedback(GenrePassiveType passiveType, Vector3 spawnPosition, int amount = 0)
    {
        FaceCamera();

        moneyText.gameObject.SetActive(false);
        popularityText.gameObject.SetActive(false);
        feedbackText.gameObject.SetActive(true);

        (string text, Color color) = passiveType switch
        {
            GenrePassiveType.DodgeChance => ("MISS", new Color32(220, 232, 242, 255)),
            GenrePassiveType.CriticalHitChance => ("CRIT", new Color32(255, 70, 70, 255)),
            GenrePassiveType.FirstAttackBonus => ("FIRST\nHIT", new Color32(180, 120, 255, 255)),
            GenrePassiveType.Lifesteal => ("LIFE\nSTEAL", new Color32(110, 165, 125, 255)),
            GenrePassiveType.LastStand => ("LAST\nSTAND", new Color32(100, 160, 155, 255)),
            GenrePassiveType.DeathRefund => ($"REFUND\n+{amount:N0}M $", new Color32(55, 125, 75, 255)),
            _ => throw new ArgumentOutOfRangeException(nameof(passiveType), passiveType, null)
        };

        feedbackText.text = text;
        feedbackText.color = color;
        feedbackText.transform.position = spawnPosition;

        StartCoroutine(AnimateVertical(feedbackText, feedbackDuration, feedbackUpDistance, feedbackFadeStart));

        Destroy(gameObject, feedbackDuration + 0.1f);
    }

    private void FaceCamera()
    {
        if (Camera.main != null)
            transform.rotation = Camera.main.transform.rotation;
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

    private static IEnumerator AnimateVertical(TextMeshPro text, float duration, float upDistance, float fadeStart)
    {
        Vector3 startPosition = text.transform.position;
        Vector3 endPosition = startPosition + Vector3.up * upDistance;

        Color baseColor = text.color;
        baseColor.a = 1f;
        text.color = baseColor;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float movementT = 1f - Mathf.Pow(1f - t, 3f);

            text.transform.position = Vector3.Lerp(startPosition, endPosition, movementT);

            float opacity = 1f;

            if (t >= fadeStart)
            {
                float fadeT = Mathf.InverseLerp(fadeStart, 1f, t);
                opacity = Mathf.Pow(1f - fadeT, 2f);
            }

            Color color = text.color;
            color.a = opacity;
            text.color = color;

            yield return null;
        }
    }
}