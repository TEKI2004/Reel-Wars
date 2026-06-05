using System;
using UnityEngine;

public class Player
{
    public BoxOffice BoxOffice { get; } = new();
    public Popularity Popularity { get; } = new();

    private GenreNode _currentGenre = GameManager.Instance.Config.Genre.StartingGenre;
    public GenreNode CurrentGenre
    {
        get => _currentGenre;
        set
        {
            _currentGenre = value;
            OnGenreChanged?.Invoke(value);
        }
    }
    public event Action<GenreNode> OnGenreChanged;

    public event Action<Player> OnGenreChoiceAvailable;
    public Player()
    {
        Popularity.OnTierChanged += _ =>
        {
            if (CurrentGenre.Children.Count > 0)
            {
                OnGenreChoiceAvailable?.Invoke(this);
            }
            Debug.Log($"Genre choice available: {CurrentGenre.Children.Count > 0}");
        };
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
        return BoxOffice.Spend(unitType.Cost);
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