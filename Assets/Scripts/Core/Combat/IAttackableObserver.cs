public interface IAttackableObserver : IAttackable
{
    void HandleObservedTargetDestroyed(IAttackable destroyedTarget);
    public void RegisterObserver(IAttackableObserver observer);
    public void UnregisterObserver(IAttackableObserver observer);
}