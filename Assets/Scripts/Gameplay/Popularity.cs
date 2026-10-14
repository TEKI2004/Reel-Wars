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

    public int Add(int points)
    {
        if (!CanGainPoints) return 0;

        int previousPoints = CurrentPoints;
        CurrentPoints = Mathf.Clamp(CurrentPoints + points, 0, Config.MaxPoints);

        int appliedPoints = CurrentPoints - previousPoints;
        if (appliedPoints == 0) return 0;

        CheckTierProgression();
        OnPointsChanged?.Invoke(CurrentPoints);

        return appliedPoints;
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