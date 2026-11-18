using System;
using UnityEngine;

public enum GenrePassiveType
{
    None,
    DodgeChance,
    CriticalHitChance,
    FirstAttackBonus,
    Lifesteal,
    Regeneration,
    LastStand,
    DeathRefund
}

[Serializable]
public struct GenrePassive
{
    public GenrePassiveType Type;

    [Min(0f)]
    public float Value;
}