using System;
using UnityEngine;

public class BoxOffice
{
    public event Action<int> OnMoneyChanged;
    public event Action<int> OnMoneySpent;
    public event Action<int> OnMoneyPenalty;
    public int Money { get; private set; } = GameManager.Instance.Config.StartingMoney;

    public void Add(int amount)
    {
        Money += amount;
        OnMoneyChanged?.Invoke(Money);
    }

    public bool CanAfford(int cost)
    {
        return Money >= cost;
    }

    public bool Spend(int cost)
    {
        if (!CanAfford(cost))
            return false;

        Money -= cost;
        OnMoneyChanged?.Invoke(Money);
        OnMoneySpent?.Invoke(cost);
        return true;
    }

    public void ApplyPenalty(int amount)
    {
        int deductedAmount = Mathf.Min(Money, amount);

        if (deductedAmount <= 0)
            return;

        Money -= deductedAmount;

        OnMoneyChanged?.Invoke(Money);
        OnMoneyPenalty?.Invoke(deductedAmount);
    }
}