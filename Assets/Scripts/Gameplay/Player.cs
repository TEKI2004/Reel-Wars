using System;
using UnityEngine;

public class Player
{
    public BoxOffice BoxOffice { get; } = new();
    public Popularity Popularity { get; } = new();
    public GenreNode CurrentGenre { get; private set; }

    public void SetStartingGenre(GenreNode genre)
    {
        CurrentGenre = genre;
    }

    public bool TryChooseGenre(GenreNode nextGenre)
    {
        if (nextGenre == null) return false;
        if (!CurrentGenre.Children.Contains(nextGenre)) return false;
        if (!nextGenre.IsAvailableFor(this)) return false;

        CurrentGenre = nextGenre;
        return true;
    }

    public bool BuyUnit(UnitType unitType)
    {
        return BoxOffice.Spend(unitType.cost);
    }

    public void ClaimReward(UnitRole victimRole, int victimCost)
    {
        StatsConfig config = GameManager.Instance.Config;

        int popularityPoints = victimRole switch
        {
            UnitRole.Melee => config.Rewards.MeleeDeathPoints,
            UnitRole.Ranged => config.Rewards.RangedDeathPoints,
            UnitRole.Heavy => config.Rewards.HeavyDeathPoints,
            _ => throw new ArgumentOutOfRangeException(nameof(victimRole))
        };
        Popularity.Add(popularityPoints);
        BoxOffice.Add(Mathf.RoundToInt(victimCost * config.Rewards.CostMultiplier));
    }

    
}