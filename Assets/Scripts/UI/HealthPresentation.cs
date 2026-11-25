using Sirenix.OdinInspector;
using UnityEngine;

public abstract class HealthPresentation : MonoBehaviour
{
    [Title("Health Feedback")]
    [Required][SerializeField] protected AttackedShaker shaker;
    [Required][SerializeField] protected FloatingUnitTag unitTagPrefab;

    public abstract Vector3 FeedbackPosition { get; }

    public void ShowDamage(int currentHealth, int maxHealth)
    {
        SetHealth(currentHealth, maxHealth);
        shaker.Shake();
    }

    public void ShowHealing(int currentHealth, int maxHealth)
    {
        SetHealth(currentHealth, maxHealth);
    }

    public void ShowLastStand(GenrePassiveType passiveType, int currentHealth, int maxHealth)
    {
        SetHealth(currentHealth, maxHealth);
        shaker.Shake();
        ShowFeedback(passiveType);
    }

    public void ShowFeedback(GenrePassiveType passiveType, int amount = 0)
    {
        ShowFeedback(passiveType, FeedbackPosition, amount);
    }

    public void ShowFeedback(GenrePassiveType passiveType, Vector3 position, int amount = 0)
    {
        FloatingUnitTag unitTag = CreateUnitTag();
        unitTag.ShowFeedback(passiveType, position, amount);
    }

    protected FloatingUnitTag CreateUnitTag()
    {
        return Instantiate(unitTagPrefab);
    }

    protected abstract void SetHealth(int currentHealth, int maxHealth);
}