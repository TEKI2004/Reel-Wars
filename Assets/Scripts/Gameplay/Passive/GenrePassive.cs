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

    public string Description
    {
        get
        {
            int percent = Mathf.RoundToInt(Value * 100f);

            return Type switch
            {
                GenrePassiveType.None => "―――――――――",
                GenrePassiveType.DodgeChance => $"{percent}% chance to dodge enemy attacks",
                GenrePassiveType.CriticalHitChance => $"{percent}% chance to double the damage",
                GenrePassiveType.FirstAttackBonus => $"First attack deals {100 + percent}% damage",
                GenrePassiveType.Lifesteal => $"Heals for {percent}% of damage dealt",
                GenrePassiveType.Regeneration => $"Regenerates {percent}% health per second",
                GenrePassiveType.LastStand => $"{percent}% health after first lethal hit",
                GenrePassiveType.DeathRefund => $"Refunds {percent}% of the cost on death",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}