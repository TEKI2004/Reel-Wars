using System;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Stats Config")]
public class StatsConfig : ScriptableObject
{
    public BaseStatsConfig BaseStats;
    public RewardConfig Rewards;
    public PopularityConfig Popularity;
    public BoxOfficeConfig BoxOffice;
    public GenreConfig Genre;

    private void OnValidate()
    {
        Popularity.RefreshLimits();
    }
}

[Serializable]
public class BaseStatsConfig
{
    public int BaseHealth;
}

[Serializable]
public class RewardConfig
{
    public int MeleeDeathPoints;
    public int RangedDeathPoints;
    public int HeavyDeathPoints;
    public float CostMultiplier;
}

[Serializable]
public class PopularityConfig
{
    [Header("Starting Values")]
    public int StartingPoints;
    public int StartingTier;

    [Header("Max Values")]
    public int MaxTier;
    public int MaxPoints;

    [Header("Tier Limit Growth")]
    public int BaseLimit;
    public int LinearLimitGrowth;
    public int QuadraticLimitGrowth;

    [Header("Debug Tier Limits")]
    [SerializeField] private int tier1Limit;
    [SerializeField] private int tier2Limit;

    private int GetLimit(int tier)
    {
        return BaseLimit
             + tier * LinearLimitGrowth
             + tier * tier * QuadraticLimitGrowth;
    }

    public void RefreshLimits()
    {
        tier1Limit = GetLimit(1);
        tier2Limit = GetLimit(2);
        MaxPoints = GetLimit(MaxTier);
    }
}

[Serializable]
public class BoxOfficeConfig
{
    public int StartingMoney;
}

[Serializable]
public class GenreConfig
{
    public GenreNode StartingGenre;
}