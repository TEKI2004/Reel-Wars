using Unity.VisualScripting;
using UnityEngine;

public class Popularity
{
    public event System.Action<int> OnPointsChanged;
    public int CurrentPoints { get; private set; } = GameManager.Instance.Config.startingPoints;
    public int CurrentTier { get; private set; } = GameManager.Instance.Config.startingTier;

    public void Add(int points)
    {
        CurrentPoints += points;
        OnPointsChanged?.Invoke(CurrentPoints);

        while (CurrentPoints >= GetPointsForNextTier(CurrentTier))
        {
            CurrentTier++;
        }
    }

    public int GetPointsForNextTier(int currentTier)
    {
        return GameManager.Instance.Config.baseLimit
         + currentTier * GameManager.Instance.Config.linearLimitGrowth
         + currentTier * currentTier * GameManager.Instance.Config.quadraticLimitGrowth;
    }

}
