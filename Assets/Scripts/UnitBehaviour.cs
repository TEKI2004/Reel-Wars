using System.Collections.Generic;
using UnityEngine;

public enum UnitDirection
{
    Left,
    Right
}

public class UnitBehaviour : MonoBehaviour
{
    private static float default_rangeY = 1f;
    private static float default_rangeZ = 1f;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;

    [SerializeField] private BoxCollider detectionRangeCollider;

    private int currentHealth;
    private float lastAttackTime;

    private UnitBehaviour currentTarget;
    private readonly List<UnitBehaviour> enemiesInRange = new();


    public int OwnerId { get; private set; }

    public UnitDirection direction = UnitDirection.Right;
    

    private Vector3 DirectionVector 
    {
        get
        {
            return direction == UnitDirection.Right ? Vector3.right : Vector3.left;
        }
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Initialize(int ownerId, UnitDirection moveDirection)
    {
        OwnerId = ownerId;
        direction = moveDirection;
        ConfigureDetectionRange();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0) return;

        CleanupEnemyList();

        if (currentTarget == null || !enemiesInRange.Contains(currentTarget))
        {
            currentTarget = GetNextTarget();
        }

        if (currentTarget != null)
        {
            Attack(currentTarget);
        }
        else
        {
            MoveForward();
        }
    }

    private void ConfigureDetectionRange()
    {
        detectionRangeCollider.size = new Vector3(attackRange, default_rangeY, default_rangeZ);
        detectionRangeCollider.center = attackRange * 0.5f * DirectionVector;
    }

    private void MoveForward()
    {
        transform.Translate(moveSpeed * Time.deltaTime * DirectionVector);
    }

    private UnitBehaviour GetNextTarget()
    {
        return enemiesInRange.Count > 0 ? enemiesInRange[0] : null;
    }

    private void Attack(UnitBehaviour target)
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        target.TakeDamage(damage);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void AddEnemyToRange(UnitBehaviour enemy)
    {
        if (enemy == null) return;
        if (enemy.OwnerId == OwnerId) return;
        if (!enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    public void RemoveEnemyFromRange(UnitBehaviour enemy)
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
