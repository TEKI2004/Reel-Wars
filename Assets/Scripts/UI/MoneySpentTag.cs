using UnityEngine;
using UnityEngine.UIElements;

public class MoneySpentTag
{
    private readonly VisualElement container;

    private readonly Vector2 endOffset = new(12f, 35f);
    private readonly float endRotation = -6f;

    private readonly float duration = 0.6f;
    private readonly float fadeStart = 0.3f;

    private readonly float startOpacity = 0.75f;

    private readonly Color spentColor = new Color32(196, 147, 63, 255);
    private readonly Color penaltyColor = new Color32(220, 60, 60, 255);

    public MoneySpentTag(VisualElement container)
    {
        this.container = container;
    }

    public void Show(int amount)
    {
        ShowTag(amount, spentColor);
    }

    public void ShowPenalty(int amount)
    {
        ShowTag(amount, penaltyColor);
    }

    public void Clear()
    {
        container.Clear();
    }

    private void ShowTag(int amount, Color color)
    {
        Label tag = new($"-{amount:N0}M $");

        tag.AddToClassList("money-value");
        tag.AddToClassList("money-spent-tag");

        tag.style.color = color;
        tag.style.opacity = startOpacity;

        tag.style.position = Position.Absolute;
        tag.style.left = StyleKeyword.Auto;
        tag.style.top = StyleKeyword.Auto;
        tag.style.right = 0;
        tag.style.bottom = 0;

        tag.style.translate = new Translate(0, 0);
        tag.style.rotate = new Rotate(0);

        container.Add(tag);

        Animate(tag);
    }

    private void Animate(Label tag)
    {
        float startTime = Time.unscaledTime;
        IVisualElementScheduledItem animation = null;

        animation = tag.schedule.Execute(() =>
        {
            float elapsed = Time.unscaledTime - startTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float movementT = 1f - Mathf.Pow(1f - t, 3f);

            Vector2 offset = endOffset * movementT;
            tag.style.translate = new Translate(offset.x, offset.y);

            float rotation = endRotation * movementT;
            tag.style.rotate = new Rotate(rotation);

            float opacity = startOpacity;

            if (t >= fadeStart)
            {
                float fadeT = Mathf.InverseLerp(fadeStart, 1f, t);
                opacity = startOpacity * Mathf.Pow(1f - fadeT, 2f);
            }

            tag.style.opacity = opacity;

            if (t >= 1f)
            {
                animation.Pause();
                tag.RemoveFromHierarchy();
            }
        }).Every(16);
    }
}