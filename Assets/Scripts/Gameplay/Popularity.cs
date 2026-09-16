using System;
using UnityEngine;

public class Popularity
{
    public event Action<int> OnPointsChanged;
    public event Action<int> OnTierChanged;

    public int CurrentPoints { get; private set; }
    public int CurrentTier { get; private set; }

    private PopularityConfig Config => GameManager.Instance.Config.Popularity;

    public Popularity()
    {
        CurrentPoints = Config.StartingPoints;
        CurrentTier = 1;
    }

    public void Add(int points)
    {
        CurrentPoints += points;
        OnPointsChanged?.Invoke(CurrentPoints);

        CheckTierProgression();
    }

    public void CheckTierProgression()
    {
        while (
            CurrentTier < Config.MaxTier &&
            CurrentPoints >= Config.GetTierLimit(CurrentTier)
        )
        {
            CurrentTier++;

            Debug.Log($"Popularity tier increased to {CurrentTier}");

            OnTierChanged?.Invoke(CurrentTier);

        }
    }

}
