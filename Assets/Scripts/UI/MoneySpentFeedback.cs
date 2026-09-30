using UnityEngine;
using UnityEngine.UIElements;

public class MoneySpentFeedback
{
    private readonly VisualElement container;
    private readonly MoneySpentAnimationConfig config;

    public MoneySpentFeedback(VisualElement container, MoneySpentAnimationConfig config)
    {
        this.container = container;
        this.config = config;
    }

    public void Show(int amount)
    {
        Label tag = CreateTag(amount);

        container.Add(tag);

        Animate(tag);
    }

    public void Clear()
    {
        container.Clear();
    }


    // ─────────────────────────────────────────────
    // Tag Creation
    // ─────────────────────────────────────────────

    private Label CreateTag(int amount)
    {
        Label tag = new($"-{amount:N0}M $");

        tag.AddToClassList("money-value");
        tag.AddToClassList("money-spent-tag");

        tag.style.color = config.Color;
        tag.style.opacity = config.StartOpacity;

        tag.style.position = Position.Absolute;

        tag.style.left = StyleKeyword.Auto;
        tag.style.top = StyleKeyword.Auto;

        tag.style.right = 0;
        tag.style.bottom = 0;

        tag.style.translate = new Translate(0,0);

        tag.style.rotate = new Rotate(0);

        return tag;
    }


    // ─────────────────────────────────────────────
    // Animation
    // ─────────────────────────────────────────────

    private void Animate(Label tag)
    {
        float startTime = Time.unscaledTime;

        IVisualElementScheduledItem animation = null;

        animation = tag.schedule.Execute(() =>
        {
            float elapsed =Time.unscaledTime - startTime;

            float t = Mathf.Clamp01(elapsed / config.Duration);

            // ─────────────────────────────────────
            // Movement
            // Fast start → slow finish
            // ─────────────────────────────────────

            float movementT = 1f - Mathf.Pow(1f - t, 3f);

            Vector2 offset = config.EndOffset * movementT;

            tag.style.translate = new Translate(offset.x, offset.y);

            // ─────────────────────────────────────
            // Rotation
            // ─────────────────────────────────────

            float rotation = config.EndRotation * movementT;

            tag.style.rotate = new Rotate(rotation);

            // ─────────────────────────────────────
            // Opacity
            // ─────────────────────────────────────

            float opacity = config.StartOpacity;

            if (t >= config.FadeStart)
            {
                float fadeT = Mathf.InverseLerp(config.FadeStart,1f,t);

                opacity = config.StartOpacity * Mathf.Pow(1f - fadeT, 2f);
            }

            tag.style.opacity = opacity;


            // ─────────────────────────────────────
            // Cleanup
            // ─────────────────────────────────────

            if (t >= 1f)
            {
                animation.Pause();
                tag.RemoveFromHierarchy();
            }
        })
        .Every(16);
    }
}