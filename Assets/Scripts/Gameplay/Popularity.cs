using System;
using UnityEngine;

public class Popularity
{
    public event Action<int> OnPointsChanged;
    public event Action<int> OnTierChanged;

    public int CurrentPoints { get; private set; } = GameManager.Instance.Config.Popularity.StartingPoints;
    public int CurrentTier { get; private set; } = GameManager.Instance.Config.Popularity.StartingTier;

    public void Add(int points)
    {
        CurrentPoints += points;
        OnPointsChanged?.Invoke(CurrentPoints);

        while (CurrentTier < GameManager.Instance.Config.Popularity.MaxTier && CurrentPoints >= GetLimitToLeaveTier(CurrentTier))
        {
            CurrentTier++;
            OnTierChanged?.Invoke(CurrentTier);
            Debug.Log($"Popularity tier increased to {CurrentTier}");
        }
    }

    public static int[] GetPopularityThresholds()
    {
        StatsConfig config = GameManager.Instance.Config;
        int[] limits = new int[config.Popularity.MaxTier - 1];
        for (int tier = 1; tier < config.Popularity.MaxTier; tier++)
        {
            limits[tier - 1] = GetLimitToLeaveTier(tier);
        }
        return limits;
    }

    public static int GetLimitToLeaveTier(int tier)
    {
        StatsConfig config = GameManager.Instance.Config;

        return config.Popularity.BaseLimit
         + tier * config.Popularity.LinearLimitGrowth
         + tier * tier * config.Popularity.QuadraticLimitGrowth;
    }

}
