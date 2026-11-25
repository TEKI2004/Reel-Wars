using UnityEngine;

public sealed class UnitPassive
{
    private readonly GenrePassiveType type;
    private readonly float value;

    private float regenerationProgress;
    private bool lastStandUsed;

    public GenrePassiveType Type => type;

    public UnitPassive(GenrePassive passive)
    {
        type = passive.Type;
        value = passive.Value;
    }

    public bool TryDodge()
    {
        return type == GenrePassiveType.DodgeChance && Random.value < Mathf.Clamp01(value);
    }

    public int ModifyDamage(int baseDamage, bool isFirstAttack, out bool activated)
    {
        activated = false;

        if (type == GenrePassiveType.CriticalHitChance)
        {
            activated = Random.value < Mathf.Clamp01(value);
            return activated ? baseDamage * 2 : baseDamage;
        }

        if (type == GenrePassiveType.FirstAttackBonus && isFirstAttack)
        {
            activated = true;
            return Mathf.RoundToInt(baseDamage * (1f + value));
        }

        return baseDamage;
    }

    public int GetLifestealHealing(int appliedDamage)
    {
        if (type != GenrePassiveType.Lifesteal)
            return 0;

        return Mathf.RoundToInt(appliedDamage * Mathf.Clamp01(value));
    }

    public int GetRegenerationHealing(int maxHealth, bool hasFullHealth, float deltaTime)
    {
        if (type != GenrePassiveType.Regeneration)
            return 0;

        if (hasFullHealth)
        {
            regenerationProgress = 0f;
            return 0;
        }

        regenerationProgress += maxHealth * value * deltaTime;

        int healing = Mathf.FloorToInt(regenerationProgress);
        regenerationProgress -= healing;

        return healing;
    }

    public bool TryLastStand(int maxHealth, out int restoredHealth)
    {
        restoredHealth = 0;

        if (type != GenrePassiveType.LastStand || lastStandUsed)
            return false;

        lastStandUsed = true;
        restoredHealth = Mathf.Max(1, Mathf.RoundToInt(maxHealth * Mathf.Clamp01(value)));

        return true;
    }

    public int GetDeathRefund(int cost)
    {
        if (type != GenrePassiveType.DeathRefund)
            return 0;

        return Mathf.RoundToInt(cost * Mathf.Clamp01(value));
    }
}