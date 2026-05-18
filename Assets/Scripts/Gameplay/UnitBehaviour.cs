using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection
{
    Left,
    Right
}

public class UnitBehaviour : MonoBehaviour, IAttackableObserver
{
    private static float default_rangeY = 1f;
    private static float default_rangeZ = 1f;
    private static float blockRange = 0.7f;
    private static int unitNumber = 0;
    public int UnitId { get; private set; }

    [SerializeField] private UnitType unitType;

    [SerializeField] private BoxCollider attackRangeDetectorCollider;
    [SerializeField] private BoxCollider moveRangeDetectorCollider;

    private int currentHealth;
    private float lastAttackTime;

    private IAttackable currentTarget;
    private readonly List<IAttackable> enemiesInRange = new();
    private readonly List<IAttackable> blockingEntitiesInRange = new();
    private readonly HashSet<IAttackableObserver> observers = new();

    public string OwnerId { get; private set; }

    private FacingDirection direction = FacingDirection.Right;
    
    private Vector3 FacingDirectionVector 
    {
        get
        {
            return direction == FacingDirection.Left ? Vector3.left : Vector3.right;
        }
    }

    public void Initialize(string ownerId, UnitType unitType, FacingDirection moveDirection)
    {
        OwnerId = ownerId;
        UnitId = unitNumber++;

        this.unitType = unitType;

        gameObject.name = $"{OwnerId}:U{UnitId}({unitType.UnitName})";
        
        direction = moveDirection;
        currentHealth = unitType.MaxHealth;
        lastAttackTime = -unitType.AttackCooldown;
        ConfigureDetectors();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0) return;

        if (currentTarget == null)
        {
            currentTarget = GetNextTarget();
        }

        if (currentTarget != null)
        {
            Attack();
        }

        if (!HasBlockingEntity())
        {
            MoveForward();
        }
    }

    private void ConfigureDetectors()
    {
        attackRangeDetectorCollider.size = new Vector3(unitType.AttackRange, default_rangeY, default_rangeZ);
        attackRangeDetectorCollider.center = unitType.AttackRange * 0.5f * FacingDirectionVector;

        moveRangeDetectorCollider.size = new Vector3(blockRange, default_rangeY, default_rangeZ);
        moveRangeDetectorCollider.center = blockRange * 0.5f * FacingDirectionVector;
    }

    private bool HasBlockingEntity()
    {
        return blockingEntitiesInRange.Count != 0;
    }

    private void MoveForward()
    {
        transform.Translate(unitType.MoveSpeed * Time.deltaTime * FacingDirectionVector, Space.World);
    }

    private IAttackable GetNextTarget()
    {
        return enemiesInRange.Count > 0 ? enemiesInRange[0] : null;
    }

    private void Attack()
    {
        if (!enemiesInRange.Contains(currentTarget))
        {
            currentTarget = null;
            return;
        }

        if (Time.time < lastAttackTime + unitType.AttackCooldown) return;

        lastAttackTime = Time.time;
        currentTarget.TakeDamage(unitType.Damage);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameManager.Instance.GiveReward(direction, unitType.Role, unitType.Cost);

            currentTarget = null;
            enemiesInRange.Clear();
            blockingEntitiesInRange.Clear();

            NotifyDeath();
            Destroy(gameObject);
        }

        Debug.Log($"{OwnerId}:U{UnitId}({unitType.UnitName}) took {amount} damage, current health: {currentHealth}");
    }

    private void NotifyDeath()
    {
        foreach (var observer in observers)
        {
            if (observer != null)
            {
                observer.HandleObservedTargetDestroyed(this);
            }
        }

        observers.Clear();
    }

    public void RegisterObserver(IAttackableObserver observer)
    {
        if (observer == null) return;
        observers.Add(observer);
    }

    public void UnregisterObserver(IAttackableObserver observer)
    {
        if (observer == null) return;
        observers.Remove(observer);
    }

    public void HandleObservedTargetDestroyed(IAttackable destroyedTarget)
    {
        if (currentTarget == destroyedTarget)
        {
            currentTarget = null;
        }
        enemiesInRange.Remove(destroyedTarget);
        blockingEntitiesInRange.Remove(destroyedTarget);
    }

    public void HandleEnemyEnter(IAttackable enemy)
    {
        if (enemy == null) return;
        if (enemy.OwnerId == OwnerId) return;

        if (!enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
            var observable = enemy as IAttackableObserver;
            observable?.RegisterObserver(this);
        }
    }

    public void HandleEnemyExit(IAttackable enemy)
    {
        if (enemy == null) return;

        enemiesInRange.Remove(enemy);
        var observable = enemy as IAttackableObserver;
        observable?.UnregisterObserver(this);

        if (currentTarget == enemy)
            currentTarget = null;
    }

    public void HandleBlockEnter(IAttackable entity)
    {
        if (entity == null) return;

        if (!blockingEntitiesInRange.Contains(entity))
        {
            blockingEntitiesInRange.Add(entity);
            var observable = entity as IAttackableObserver;
            observable?.RegisterObserver(this);
        }
    }

    public void HandleBlockExit(IAttackable entity)
    {
        if (entity == null) return;

        blockingEntitiesInRange.Remove(entity);
        var observable = entity as IAttackableObserver;
        observable?.UnregisterObserver(this);
    }
}
