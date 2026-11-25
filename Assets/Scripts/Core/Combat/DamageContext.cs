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