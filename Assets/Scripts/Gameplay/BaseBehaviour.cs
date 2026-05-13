using UnityEngine;

public class BaseBehaviour : MonoBehaviour, IAttackable
{
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private GameObject unitPrefab;

    public string OwnerId { get; private set; }
    public Player Player { get; private set; }

    private FacingDirection facingDirection;
    private int maxHealth;
    private int currentHealth;

    public void Initialize(string ownerId, FacingDirection direction)
    {
        OwnerId = ownerId;
        Player = new Player();
        gameObject.name = $"Base_{OwnerId}";

        facingDirection = direction;
        maxHealth = GameManager.Instance.Config.BaseStats.BaseHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log($"{gameObject.name} has been destroyed.");
        }

        Debug.Log($"{gameObject.name} took {amount} damage. Current health: {currentHealth}/{maxHealth}");
    }

    public void SpawnUnit(UnitType unitType)
    {
        if (unitType == null)
        {
            Debug.LogError("UnitType is null!");
            return;
        }

        if (IsSpawnAreaBlocked())
        {
            Debug.LogWarning($"{OwnerId}'s spawn area is occupied. Cannot spawn unit.");
            return;
        }

        if (!Player.BuyUnit(unitType))
        {
            Debug.LogWarning($"{OwnerId} cannot buy unit.");
            return;
        }

        GameObject unitObject = Instantiate(unitPrefab, spawnPoint.position, Quaternion.identity);
        UnitBehaviour unit = unitObject.GetComponent<UnitBehaviour>();
        if (unit != null)
        {
            unit.Initialize(OwnerId, unitType, facingDirection);
        }

        Debug.Log($"{OwnerId} Spawned {unitType.unitName}");
    }

    public bool IsSpawnAreaBlocked()
    {
        Collider[] hits = Physics.OverlapBox(
            spawnArea.bounds.center,
            spawnArea.bounds.extents,
            Quaternion.identity,
            unitLayer
        );

        foreach (var hit in hits)
        {
            var attackable = hit.GetComponent<IAttackable>();
            if (attackable != null && attackable.OwnerId == OwnerId)
            {
                return true;
            }
        }

        return false;
    }

    public void ClaimReward(UnitRole victimRole, int victimCost)
    {
        Player.ClaimReward(victimRole, victimCost);
    }
}
