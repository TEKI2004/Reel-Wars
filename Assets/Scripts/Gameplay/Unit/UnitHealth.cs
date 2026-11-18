using UnityEngine;

public sealed class UnitHealth
{
    private readonly UnitType unitType;
    private readonly UnitPassive passive;
    private readonly UnitPresentation presentation;

    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    public UnitHealth(UnitType unitType, UnitPassive passive, UnitPresentation presentation)
    {
        this.unitType = unitType;
        this.passive = passive;
        this.presentation = presentation;

        CurrentHealth = unitType.MaxHealth;
    }

    public int TakeDamage(int amount, out bool died)
    {
        died = false;

        if (IsDead || amount <= 0)
            return 0;

        if (passive.TryDodge())
        {
            presentation.ShowFeedback(passive.Type);
            return 0;
        }

        int appliedDamage = Mathf.Min(amount, CurrentHealth);
        CurrentHealth -= appliedDamage;

        if (CurrentHealth > 0)
        {
            presentation.ShowDamage(CurrentHealth, unitType.MaxHealth);
        }
        else if (passive.TryLastStand(unitType.MaxHealth, out int restoredHealth))
        {
            CurrentHealth = restoredHealth;
            presentation.ShowLastStand(passive.Type, CurrentHealth, unitType.MaxHealth);
        }
        else
        {
            died = true;
        }

        return appliedDamage;
    }

    public void ApplyLifesteal(int appliedDamage)
    {
        int appliedHealing = Heal(passive.GetLifestealHealing(appliedDamage));

        if (appliedHealing > 0)
            presentation.ShowFeedback(passive.Type);
    }

    public void ApplyRegeneration(float deltaTime)
    {
        int healing = passive.GetRegenerationHealing(unitType.MaxHealth, CurrentHealth >= unitType.MaxHealth, deltaTime);

        if (healing > 0)
            Heal(healing);
    }

    private int Heal(int amount)
    {
        if (amount <= 0 || IsDead || CurrentHealth >= unitType.MaxHealth)
            return 0;

        int appliedHealing = Mathf.Min(amount, unitType.MaxHealth - CurrentHealth);
        CurrentHealth += appliedHealing;

        presentation.ShowHealing(CurrentHealth, unitType.MaxHealth);

        return appliedHealing;
    }
}