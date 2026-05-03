using System;
using UnityEngine;

public class Player
{
    public BoxOffice BoxOffice { get; } = new();
    public Popularity Popularity { get; } = new();

    public bool BuyUnit(UnitType unitType)
    {
        return BoxOffice.Spend(unitType.cost);
    }

    public void ClaimReward(UnitRole victimRole, int victimCost)
    {
        int popularityPoints = victimRole switch
        {
            UnitRole.Melee => GameManager.Instance.Config.meleeDeathPoints,
            UnitRole.Ranged => GameManager.Instance.Config.rangedDeathPoints,
            UnitRole.Heavy => GameManager.Instance.Config.heavyDeathPoints,
            _ => throw new ArgumentOutOfRangeException(nameof(victimRole))
        };
        Popularity.Add(popularityPoints);
        BoxOffice.Add(Mathf.RoundToInt(victimCost * GameManager.Instance.Config.costMultiplier));
    }
}