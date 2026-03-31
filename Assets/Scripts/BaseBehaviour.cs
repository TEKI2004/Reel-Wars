using UnityEngine;

public class BaseBehaviour : MonoBehaviour, IAttackable
{

    [SerializeField] private GameManager gameManager;
    [SerializeField] private FacingDirection facingDirection = FacingDirection.Right;
    [SerializeField] private int maxHealth = 500;

    private int currentHealth;

    public string OwnerId { get; private set; }

    private void Awake()
    {
        if (facingDirection == FacingDirection.Right)
        {
            OwnerId = gameManager.LeftPlayerName;
        }
        else
        {
            OwnerId = gameManager.RightPlayerName;
        }

        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Current health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} has been destroyed.");
        }
    }
}
