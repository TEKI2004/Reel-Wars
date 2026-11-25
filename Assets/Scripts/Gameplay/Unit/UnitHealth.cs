using UnityEngine;

public sealed class UnitHealth : Health
{
    private readonly UnitPassive passive;
    private readonly UnitPresentation presentation;

    public UnitHealth(UnitType unitType, UnitPassive passive, UnitPresentation presentation) : base(unitType.MaxHealth)
    {
        this.passive = passive;
        this.presentation = presentation;
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

        int appliedDamage = ApplyDamage(amount);

        if (CurrentHealth > 0)
        {
            presentation.ShowDamage(CurrentHealth, MaxHealth);
        }
        else if (passive.TryLastStand(MaxHealth, out int restoredHealth))
        {
            SetHealth(restoredHealth);
            presentation.ShowLastStand(passive.Type, CurrentHealth, MaxHealth);
        }
        else
        {
            died = true;
        }

        return appliedDamage;
    }

    public void ApplyLifesteal(int appliedDamage)
    {
        int healing = passive.GetLifestealHealing(appliedDamage);
        int appliedHealing = Heal(healing);

        if (appliedHealing > 0)
            presentation.ShowLifesteal(CurrentHealth, MaxHealth);
    }

    public void ApplyRegeneration(float deltaTime)
    {
        int healing = passive.GetRegenerationHealing(MaxHealth, CurrentHealth >= MaxHealth, deltaTime);
        int appliedHealing = Heal(healing);

        if (appliedHealing > 0)
        {
            presentation.ShowHealing(CurrentHealth, MaxHealth);
            if (CurrentHealth == MaxHealth)
                Debug.Log($"Fully regenerated to {MaxHealth} HP.");
        }
    }
}