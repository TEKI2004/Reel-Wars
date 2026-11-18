using UnityEngine;

public class BaseBehaviour : MonoBehaviour, IAttackable
{
    [Header("Neon")]
    [SerializeField] private Material neonOnMaterial;
    [SerializeField] private Material neonOffMaterial;
    [SerializeField] private NeonPlayerName playerNameDisplay;
    [SerializeField] private Material rewardNeonMaterial;
    public Material NeonMaterial => rewardNeonMaterial;

    [Header("Spawn Settings")]
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private LayerMask unitLayer;


    [Header("Reactive Effects")]
    [SerializeField] private AttackedShaker baseShaker;
    [SerializeField] private BaseEmissionFader baseEmissionFader;

    [SerializeField] private SpawnAreaBlocked spawnAreaBlocked;

    [Header("Feedback")]
    [SerializeField] private Transform hitFeedbackPoint;
    public Vector3 FeedbackPosition => hitFeedbackPoint.position;

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
        maxHealth = GameManager.Instance.Config.BaseHealth;
        currentHealth = maxHealth;

        playerNameDisplay.Initialize(neonOnMaterial, neonOffMaterial);
        playerNameDisplay.SetName(OwnerId);
    }


    public int TakeDamage(int amount)
    {
        if (currentHealth <= 0)
            return 0;

        int appliedDamage = Mathf.Min(amount, currentHealth);
        currentHealth -= appliedDamage;

        playerNameDisplay.SetLitPercent((float)currentHealth / maxHealth);
        baseEmissionFader.SetLitPercent((float)currentHealth / maxHealth);
        baseShaker.Shake();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log($"{gameObject.name} has been destroyed.");
        }

        Debug.Log($"{gameObject.name} took {amount} damage. Current health: {currentHealth}/{maxHealth}");
        return appliedDamage;
    }

    public void SpawnUnit(UnitType unitType)
    {
        if (unitType == null)
        {
            Debug.LogError("UnitType is null.");
            return;
        }

        if (unitType.UnitPrefab == null)
        {
            Debug.LogError($"Unit '{unitType.Id}' has no UnitPrefab.");
            return;
        }

        if (IsSpawnAreaBlocked())
        {
            int penalty = GameManager.Instance.Config.SpamPenaltyPerTier * Player.Popularity.CurrentTier;

            Player.BoxOffice.ApplyPenalty(penalty);
            spawnAreaBlocked.Flash();

            Debug.Log($"{OwnerId}'s spawn area is occupied. Spam penalty: {penalty}M $");
            return;
        }

        if (!Player.BuyUnit(unitType))
        {
            Debug.Log($"{OwnerId} cannot buy {unitType.UnitName} for {unitType.Cost}M $.");
            return;
        }

        CreateUnit(unitType);

        Debug.Log($"{OwnerId} spawned {unitType.UnitName}.");
    }

    private UnitBehaviour CreateUnit(UnitType unitType)
    {
        Quaternion rotation = facingDirection == FacingDirection.Right ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);

        GameObject unitObject = Instantiate(unitType.UnitPrefab, spawnPoint.position, rotation);

        UnitBehaviour unit = unitObject.GetComponent<UnitBehaviour>();

        unit.Initialize(OwnerId, unitType, Player.CurrentGenre.Passive);
        unit.OnDeathRefund += Player.BoxOffice.Add;

        if (facingDirection == FacingDirection.Left)
            unit.MirrorVisual();

        return unit;
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

    public RewardResult ClaimReward(UnitRole victimRole, int victimCost)
    {
        return Player.ClaimReward(victimRole, victimCost);
    }
}
