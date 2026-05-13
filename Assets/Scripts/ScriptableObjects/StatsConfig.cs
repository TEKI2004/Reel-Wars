using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Stats Config")]
public class StatsConfig : ScriptableObject
{
    public BaseStatsConfig BaseStats;
    public RewardConfig Rewards;
    public PopularityConfig Popularity;
    public BoxOfficeConfig BoxOffice;

    private void OnValidate()
    {
        Popularity.RefreshDebugLimits();
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
    public int StartingPoints;
    public int StartingTier;

    public int BaseLimit;
    public int LinearLimitGrowth;
    public int QuadraticLimitGrowth;

    [Header("Debug Tier Limits")]
    [SerializeField] private int tier1;
    [SerializeField] private int tier2;
    [SerializeField] private int tier3;
    [SerializeField] private int tier4;

    private int GetLimit(int tier)
    {
        return BaseLimit
             + tier * LinearLimitGrowth
             + tier * tier * QuadraticLimitGrowth;
    }

    public void RefreshDebugLimits()
    {
        tier1 = GetLimit(1);
        tier2 = GetLimit(2);
        tier3 = GetLimit(3);
        tier4 = GetLimit(4);
    }
}

[Serializable]
public class BoxOfficeConfig
{
    public int StartingMoney;
}