using UnityEngine;

public interface IAttackable 
{
    string OwnerId { get; }
    int TakeDamage(DamageContext context);
    Vector3 FeedbackPosition { get; }
}

public readonly struct DamageContext
{
    public int Amount { get; }
    public UnitBehaviour Source { get; }

    public DamageContext(int amount, UnitBehaviour source)
    {
        Amount = amount;
        Source = source;
    }
}