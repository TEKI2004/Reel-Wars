using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection
{
    Left,
    Right
}

public class UnitBehaviour : MonoBehaviour, IAttackable
{
    private static float default_rangeY = 1f;
    private static float default_rangeZ = 1f;
    private static float blockRange = 1.1f;
    private static int unitNumber = 0;

    private int unitId;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;

    [SerializeField] private BoxCollider attackRangeDetectorCollider;
    [SerializeField] private BoxCollider moveRangeDetectorCollider;

    private int currentHealth;
    private float lastAttackTime;

    private IAttackable currentTarget;
    private readonly List<IAttackable> enemiesInRange = new();
    private readonly List<IAttackable> blockingEntitiesInRange = new();

    public string OwnerId { get; private set; }

    public FacingDirection direction = FacingDirection.Right;
    
    private Vector3 FacingDirectionVector 
    {
        get
        {
            return direction == FacingDirection.Right ? Vector3.right : Vector3.left;
        }
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Initialize(string ownerId, FacingDirection moveDirection)
    {
        OwnerId = ownerId;
        unitId = unitNumber++;
        direction = moveDirection;
        ConfigureDetectors();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0) return;

        CleanupEnemyList();

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
        attackRangeDetectorCollider.size = new Vector3(attackRange, default_rangeY, default_rangeZ);
        attackRangeDetectorCollider.center = attackRange * 0.5f * FacingDirectionVector;

        moveRangeDetectorCollider.size = new Vector3(blockRange, default_rangeY, default_rangeZ);
        moveRangeDetectorCollider.center = blockRange * 0.5f * FacingDirectionVector;
    }

    private bool HasBlockingEntity()
    {
        blockingEntitiesInRange.RemoveAll(e => e == null);

        foreach (var entity in blockingEntitiesInRange)
        {
            if (entity.OwnerId == OwnerId) 
            { 
                Debug.Log($"For {OwnerId}:U{unitId} Ignoring blocking entity in range: {entity.OwnerId} (same owner)");
            } 
            else
            {
                Debug.Log($"For {OwnerId}:U{unitId} Blocking entity in range: {entity.OwnerId}");
            }
            
            return true;
        }

        return blockingEntitiesInRange.Count != 0;
    }

    private void MoveForward()
    {
        transform.Translate(moveSpeed * Time.deltaTime * FacingDirectionVector, Space.World);
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

        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        bool hasDied = currentTarget.TakeDamage(damage);
        if (hasDied)
        {
            blockingEntitiesInRange.Remove(currentTarget);
            enemiesInRange.Remove(currentTarget);
            currentTarget = null;
        }
    }

    public bool TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{OwnerId} took {amount} damage, current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentTarget = null;
            enemiesInRange.Clear();
            Destroy(gameObject);
            return true;
        }

        return false;
    }

    public void AddBlockingEntityToRange(IAttackable entity)
    {
        if (entity == null || entity == (IAttackable) this) return;

        if (!blockingEntitiesInRange.Contains(entity))
        {
            blockingEntitiesInRange.Add(entity);
        }
    }

    public void RemoveBlockingEntityFromRange(IAttackable entity)
    {
        if (entity == null) return;

        blockingEntitiesInRange.Remove(entity);
    }

    public void AddEnemyToRange(IAttackable enemy)
    {
        if (!enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
            Debug.Log($"For {OwnerId} Enemy added to range: {enemy.OwnerId}");
        }
    }

    public void RemoveEnemyFromRange(IAttackable enemy)
    {
        if (enemy == null) return;
        enemiesInRange.Remove(enemy);

        if (currentTarget == enemy)
        {
            currentTarget = null;
        }
    }

    private void CleanupEnemyList()
    {
        enemiesInRange.RemoveAll(enemy => enemy == null);
    }
}
