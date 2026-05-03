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

        while (CurrentPoints >= GetPointsForNextTier())
        {
            CurrentTier++;
            Debug.Log($"Popularity tier increased to {CurrentTier}");
        }
    }

    public int GetPointsForNextTier()
    {
        return GameManager.Instance.Config.baseLimit
         + CurrentTier * GameManager.Instance.Config.linearLimitGrowth
         + CurrentTier * CurrentTier * GameManager.Instance.Config.quadraticLimitGrowth;
    }

}
