using UnityEngine;

public sealed class BaseHealth : Health
{
    private readonly BasePresentation presentation;
    private readonly BasePassive passive;

    public BaseHealth(int maxHealth, GenrePassive genrePassive, BasePresentation presentation) : base(maxHealth)
    {
        this.presentation = presentation;
        passive = new BasePassive(genrePassive);
    }

    public void SetPassive(GenrePassive genrePassive)
    {
        passive.Set(genrePassive);
    }

    public void ApplyRegeneration(float deltaTime)
    {
        if (IsDead)
            return;

        int healing = passive.GetRegenerationHealing(MaxHealth, CurrentHealth >= MaxHealth, deltaTime);
        int appliedHealing = Heal(healing);

        if (appliedHealing > 0)
        {
            presentation.ShowHealing(CurrentHealth, MaxHealth);
            if (CurrentHealth == MaxHealth)
                Debug.Log($"Fully regenerated to {MaxHealth} HP.");
        }

        
    }

    public int TakeDamage(DamageContext context)
    {
        if (IsDead || context.Amount <= 0)
            return 0;

        if (passive.TryDodge())
        {
            presentation.ShowFeedback(passive.Type);
            return 0;
        }

        int appliedDamage = ApplyDamage(context.Amount);

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
            presentation.ShowDamage(CurrentHealth, MaxHealth);
        }

        if (context.Source != null && passive.TryTriggerFirstAttackerDeath())
        {
            presentation.ShowFeedback(passive.Type);
            context.Source.TakeDamage(new DamageContext(context.Source.MaxHealth, null));
        }

        return appliedDamage;
    }
}