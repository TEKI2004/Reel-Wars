using UnityEngine;

public sealed class BasePassive
{
    private const float DodgeValueMultiplier = 0.5f;
    private const float RegenerationValueMultiplier = 0.05f;
    private const float LastStandValueMultiplier = 0.25f;

    private GenrePassiveType type;
    private float value;

    private float regenerationProgress;
    private bool lastStandUsed;
    private bool firstAttackerHandled;

    public GenrePassiveType Type => type;

    public BasePassive(GenrePassive passive)
    {
        Set(passive);
    }

    public void Set(GenrePassive passive)
    {
        type = passive.Type;
        value = passive.Value;

        regenerationProgress = 0f;
        lastStandUsed = false;
        firstAttackerHandled = false;
    }

    public bool TryDodge()
    {
        float chance = Mathf.Clamp01(value) * DodgeValueMultiplier;
        return type == GenrePassiveType.DodgeChance && Random.value < chance;
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

        regenerationProgress += maxHealth * ( Mathf.Clamp01(value) * RegenerationValueMultiplier ) * deltaTime;

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

        float healthRatio = Mathf.Clamp01(value) * LastStandValueMultiplier;
        restoredHealth = Mathf.Max(1, Mathf.RoundToInt(maxHealth * healthRatio));

        return true;
    }

    public bool TryTriggerFirstAttackerDeath()
    {
        if (type != GenrePassiveType.FirstAttackBonus || firstAttackerHandled)
            return false;

        firstAttackerHandled = true;
        return true;
    }
}