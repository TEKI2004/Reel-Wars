using UnityEngine;

public abstract class Health
{
    public int MaxHealth { get; }
    public int CurrentHealth { get; protected set; }
    public bool IsDead => CurrentHealth <= 0;

    protected Health(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    protected int ApplyDamage(int amount)
    {
        if (IsDead || amount <= 0)
            return 0;

        int appliedDamage = Mathf.Min(amount, CurrentHealth);
        CurrentHealth -= appliedDamage;

        return appliedDamage;
    }

    protected int Heal(int amount)
    {
        if (IsDead || amount <= 0 || CurrentHealth >= MaxHealth)
            return 0;

        int appliedHealing = Mathf.Min(amount, MaxHealth - CurrentHealth);
        CurrentHealth += appliedHealing;

        return appliedHealing;
    }

    protected void SetHealth(int amount)
    {
        CurrentHealth = Mathf.Clamp(amount, 0, MaxHealth);
    }
}