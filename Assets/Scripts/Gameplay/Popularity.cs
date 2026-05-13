using Unity.VisualScripting;
using UnityEngine;

public class Popularity
{
    public event System.Action<int> OnPointsChanged;
    public int CurrentPoints { get; private set; } = GameManager.Instance.Config.Popularity.StartingPoints;
    public int CurrentTier { get; private set; } = GameManager.Instance.Config.Popularity.StartingTier;

    public void Add(int points)
    {
        CurrentPoints += points;
        OnPointsChanged?.Invoke(CurrentPoints);

        while (CurrentPoints >= GetPointsForNextTier())
        {
            CurrentTier++;
            Debug.Log($"Popularity tier increased to {CurrentTier}");
        }
    }

    public int GetPointsForNextTier()
    {
        StatsConfig config = GameManager.Instance.Config;

        return config.Popularity.BaseLimit
         + CurrentTier * config.Popularity.LinearLimitGrowth
         + CurrentTier * CurrentTier * config.Popularity.QuadraticLimitGrowth;
    }

}
