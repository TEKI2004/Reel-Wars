using System;
using UnityEngine;

public class Popularity
{
    public event Action<int> OnPointsChanged;
    public event Action OnTierChanged;

    public int CurrentPoints { get; private set; }
    public int CurrentTier { get; private set; }
    public bool CanGainPoints { get; private set; }

    private PopularityConfig Config => GameManager.Instance.Config.Popularity;

    public Popularity()
    {
        CurrentPoints = Config.StartingPoints;
        CurrentTier = 1;
        CanGainPoints = true;
    }

    public void Add(int points)
    {
        if (!CanGainPoints) return;

        CurrentPoints += points;
        OnPointsChanged?.Invoke(CurrentPoints);
        CheckTierProgression();
    }

    public void SetGainEnabled(bool enabled)
    {
        CanGainPoints = enabled;
    }

    public void ResetAfterOscarSeason()
    {
        CurrentPoints = Config.GetTierLimit(Config.MaxTier - 1);
        CurrentTier = Config.MaxTier;

        OnPointsChanged?.Invoke(CurrentPoints);
    }

    public void CheckTierProgression()
    {
        while (CurrentTier < Config.MaxTier && CurrentPoints >= Config.GetTierLimit(CurrentTier))
        {
            CurrentTier++;

            Debug.Log($"Popularity tier increased to {CurrentTier}");
            OnTierChanged?.Invoke();
        }
    }
}