using UnityEngine;

public class BoxOffice
{
    public event System.Action<int> OnMoneyChanged;
    public int Money { get; private set; } = GameManager.Instance.Config.BoxOffice.StartingMoney;

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
        return true;
    }
}