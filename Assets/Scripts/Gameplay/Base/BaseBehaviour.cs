using UnityEngine;

[RequireComponent(typeof(BasePresentation))]
public class BaseBehaviour : MonoBehaviour, IAttackable
{
    private BasePresentation presentation;
    private BasePresentation Presentation => presentation ??= GetComponent<BasePresentation>();

    private BaseHealth health;

    [Header("Spawn Settings")]
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private SpawnAreaBlocked spawnAreaBlocked;

    public Material NeonMaterial => Presentation.NeonMaterial;
    public Vector3 FeedbackPosition => Presentation.FeedbackPosition;

    public string OwnerId { get; private set; }
    public Player Player { get; private set; }

    private FacingDirection facingDirection;

    public void Initialize(string ownerId, FacingDirection direction)
    {
        OwnerId = ownerId;
        Player = new Player();
        gameObject.name = $"Base_{OwnerId}";

        facingDirection = direction;

        int maxHealth = GameManager.Instance.Config.BaseHealth;

        Presentation.Initialize(OwnerId, maxHealth);
        health = new BaseHealth(maxHealth, Player.CurrentGenre.Passive, Presentation);

        Player.OnGenreChanged += HandleGenreChanged;
    }

    private void HandleGenreChanged(GenreNode genre)
    {
        health.SetPassive(genre.Passive);
    }

    private void Update()
    {
        health?.ApplyRegeneration(Time.deltaTime);
    }

    public int TakeDamage(DamageContext context)
    {
        if (health == null)
            return 0;

        int appliedDamage = health.TakeDamage(context);

        if (appliedDamage <= 0)
            return 0;

        if (health.IsDead)
            Debug.Log($"{gameObject.name} has been destroyed.");

        Debug.Log($"{gameObject.name} took {appliedDamage} damage. Current health: {health.CurrentHealth}/{health.MaxHealth}");

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

    private void OnDestroy()
    {
        if (Player != null)
            Player.OnGenreChanged -= HandleGenreChanged;
    }
}
