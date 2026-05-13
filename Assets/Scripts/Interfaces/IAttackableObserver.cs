public interface IAttackableObserver : IAttackable
{
    void HandleObservedTargetDestroyed(IAttackable destroyedTarget);
    int UnitId { get; }
    public void RegisterObserver(IAttackableObserver observer);
    public void UnregisterObserver(IAttackableObserver observer);
}