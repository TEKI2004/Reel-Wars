using System;
using UnityEngine;

public class Player
{
    public BoxOffice BoxOffice { get; } = new();
    public Popularity Popularity { get; } = new();

    private StatsConfig Config => GameManager.Instance.Config;

    private GenreNode _currentGenre = GameManager.Instance.Config.StartingGenre;

    public GenreNode CurrentGenre
    {
        get => _currentGenre;
        private set
        {
            _currentGenre = value;
            OnGenreChanged?.Invoke(value);
        }
    }

    public event Action<GenreNode> OnGenreChanged;
    public event Action OnGenreChoiceAvailable;
    private int pendingGenreChoices;
    private bool genreChoiceActive;

    public event Action<UnitType> OnUnitPurchaseFailed;

    public Player()
    {
        Popularity.OnTierChanged += HandleTierChanged;
    }

    private void HandleTierChanged()
    {
        pendingGenreChoices++;
        TryOpenGenreChoice();
    }

    private void TryOpenGenreChoice()
    {
        if (genreChoiceActive)
            return;

        if (pendingGenreChoices <= 0)
            return;

        if (CurrentGenre.Children.Count == 0)
        {
            pendingGenreChoices = 0;
            Debug.LogWarning("No more genre choices available.");
            return;
        }

        genreChoiceActive = true;
        OnGenreChoiceAvailable?.Invoke();
    }

    public void ChooseGenre(GenreNode nextGenre)
    {
        CurrentGenre = nextGenre;

        pendingGenreChoices--;
        genreChoiceActive = false;

        TryOpenGenreChoice();
    }

    public bool BuyUnit(UnitType unitType)
    {
        if (BoxOffice.Spend(unitType.Cost))
            return true;

        OnUnitPurchaseFailed?.Invoke(unitType);
        return false;
    }

    public RewardResult ClaimReward(UnitRole victimRole, int victimCost)
    {
        int popularity = victimRole switch
        {
            UnitRole.Melee => Config.Rewards.MeleeDeathPoints,
            UnitRole.Ranged => Config.Rewards.RangedDeathPoints,
            UnitRole.Heavy => Config.Rewards.HeavyDeathPoints,
            _ => throw new ArgumentOutOfRangeException(nameof(victimRole))
        };

        int awardedPopularity = Popularity.Add(popularity);
        int money = Mathf.RoundToInt(victimCost * Config.Rewards.CostMultiplier);
        BoxOffice.Add(money);

        return new RewardResult(awardedPopularity, money);
    }
}