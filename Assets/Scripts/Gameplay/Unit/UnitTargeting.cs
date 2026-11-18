using System.Collections.Generic;

public sealed class UnitTargeting
{
    private readonly IAttackableObserver owner;

    private IAttackable currentTarget;

    private readonly List<IAttackable> enemiesInRange = new();
    private readonly List<IAttackable> blockingEntitiesInRange = new();

    private readonly HashSet<IAttackableObserver> observers = new();
    private readonly HashSet<IAttackableObserver> observedTargets = new();

    public bool IsBlocked => blockingEntitiesInRange.Count > 0;

    public UnitTargeting(IAttackableObserver owner)
    {
        this.owner = owner;
    }

    public bool TryGetTarget(out IAttackable target)
    {
        if (currentTarget == null || !enemiesInRange.Contains(currentTarget))
            currentTarget = enemiesInRange.Count > 0 ? enemiesInRange[0] : null;

        target = currentTarget;
        return target != null;
    }

    public void HandleEnemyEnter(IAttackable enemy)
    {
        if (enemy == null || enemy.OwnerId == owner.OwnerId || enemiesInRange.Contains(enemy))
            return;

        enemiesInRange.Add(enemy);
        Observe(enemy);
    }

    public void HandleEnemyExit(IAttackable enemy)
    {
        if (enemy == null)
            return;

        enemiesInRange.Remove(enemy);

        if (currentTarget == enemy)
            currentTarget = null;

        StopObservingIfUnused(enemy);
    }

    public void HandleBlockEnter(IAttackable entity)
    {
        if (entity == null || blockingEntitiesInRange.Contains(entity))
            return;

        blockingEntitiesInRange.Add(entity);
        Observe(entity);
    }

    public void HandleBlockExit(IAttackable entity)
    {
        if (entity == null)
            return;

        blockingEntitiesInRange.Remove(entity);
        StopObservingIfUnused(entity);
    }

    public void HandleObservedTargetDestroyed(IAttackable destroyedTarget)
    {
        if (currentTarget == destroyedTarget)
            currentTarget = null;

        enemiesInRange.Remove(destroyedTarget);
        blockingEntitiesInRange.Remove(destroyedTarget);

        if (destroyedTarget is IAttackableObserver observable)
            observedTargets.Remove(observable);
    }

    public void RegisterObserver(IAttackableObserver observer)
    {
        if (observer != null)
            observers.Add(observer);
    }

    public void UnregisterObserver(IAttackableObserver observer)
    {
        if (observer != null)
            observers.Remove(observer);
    }

    public void HandleOwnerDeath()
    {
        foreach (IAttackableObserver observer in observers)
        {
            if (observer != null)
                observer.HandleObservedTargetDestroyed(owner);
        }

        observers.Clear();

        foreach (IAttackableObserver observedTarget in observedTargets)
        {
            if (observedTarget != null)
                observedTarget.UnregisterObserver(owner);
        }

        observedTargets.Clear();
        enemiesInRange.Clear();
        blockingEntitiesInRange.Clear();
        currentTarget = null;
    }

    private void Observe(IAttackable entity)
    {
        if (entity is IAttackableObserver observable && observedTargets.Add(observable))
            observable.RegisterObserver(owner);
    }

    private void StopObservingIfUnused(IAttackable entity)
    {
        if (enemiesInRange.Contains(entity) || blockingEntitiesInRange.Contains(entity))
            return;

        if (entity is IAttackableObserver observable && observedTargets.Remove(observable))
            observable.UnregisterObserver(owner);
    }
}