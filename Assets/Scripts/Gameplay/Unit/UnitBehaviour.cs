using Sirenix.OdinInspector;
using System;
using UnityEngine;

public enum FacingDirection
{
    Left,
    Right
}

[RequireComponent(typeof(UnitPresentation))]
public class UnitBehaviour : MonoBehaviour, IAttackableObserver
{
    private const float BlockRange = 0.7f;
    private static int unitNumber = 0;
    public int UnitId { get; private set; }
    private UnitType unitType;
    private UnitPassive passive;
    private UnitHealth health;

    private UnitPresentation presentation;
    internal UnitPresentation Presentation => presentation ??= GetComponent<UnitPresentation>();
    public Vector3 FeedbackPosition => Presentation.FeedbackPosition;
    public event Action<int> OnDeathRefund;

    private UnitTargeting targeting;
    internal UnitTargeting Targeting => targeting ??= new UnitTargeting(this);

    [Title("References")]
    [Required][SerializeField] private BoxCollider attackRangeDetectorCollider;
    [Required][SerializeField] private BoxCollider moveRangeDetectorCollider;

    private Vector3 attackDefaultCenter;
    private Vector3 moveDefaultCenter;

    private float lastAttackTime;

    public string OwnerId { get; private set; }

    private OscarSeasonController oscarSeason;

    public void Initialize(string ownerId, UnitType unitType, GenrePassive genrePassive)
    {
        OwnerId = ownerId;
        UnitId = unitNumber++;

        this.unitType = unitType;
        passive = new UnitPassive(genrePassive);
        health = new UnitHealth(unitType, passive, Presentation);
        oscarSeason = GameManager.Instance.GetOscarSeason(ownerId);

        gameObject.name = $"{OwnerId}:U{UnitId}({unitType.UnitName})";

        lastAttackTime = -unitType.AttackCooldown;

        attackDefaultCenter = attackRangeDetectorCollider.center;
        moveDefaultCenter = moveRangeDetectorCollider.center;
        ConfigureDetectors();
    }

    // Update is called once per frame
    void Update()
    {
        if (health.IsDead)
            return;

        health.ApplyRegeneration(Time.deltaTime);

        if (Targeting.TryGetTarget(out IAttackable target))
            Attack(target);

        if (!Targeting.IsBlocked)
            MoveForward();
    }

    private void ConfigureDetectors()
    {
        attackRangeDetectorCollider.size = new Vector3(
            unitType.AttackRange * attackRangeDetectorCollider.size.x,
            attackRangeDetectorCollider.size.y,
            attackRangeDetectorCollider.size.z
        );

        attackRangeDetectorCollider.center =
            attackDefaultCenter + unitType.AttackRange * 0.5f * Vector3.right;

        moveRangeDetectorCollider.size = new Vector3(
            BlockRange * moveRangeDetectorCollider.size.x,
            moveRangeDetectorCollider.size.y,
            moveRangeDetectorCollider.size.z
        );

        moveRangeDetectorCollider.center =
            moveDefaultCenter + BlockRange * 0.5f * Vector3.right;
    }

    private void MoveForward()
    {
        float moveSpeed = unitType.MoveSpeed * oscarSeason.MoveSpeedMultiplier;
        transform.Translate(moveSpeed * Time.deltaTime * Vector3.right);
    }

    private void Attack(IAttackable target)
    {
        float attackCooldown = unitType.AttackCooldown / oscarSeason.AttackSpeedMultiplier;

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        bool isFirstAttack = lastAttackTime < 0f;
        int damage = passive.ModifyDamage(unitType.Damage, isFirstAttack, out bool passiveActivated);

        lastAttackTime = Time.time;

        Vector3 targetFeedbackPosition = target.FeedbackPosition;
        int appliedDamage = target.TakeDamage(damage);

        if (appliedDamage <= 0)
            return;

        health.ApplyLifesteal(appliedDamage);

        if (!passiveActivated)
            return;

        Vector3 feedbackPosition = passive.Type == GenrePassiveType.FirstAttackBonus ? FeedbackPosition : targetFeedbackPosition;
        Presentation.ShowFeedback(passive.Type, feedbackPosition);
    }

    public int TakeDamage(int amount)
    {
        int appliedDamage = health.TakeDamage(amount, out bool died);

        if (appliedDamage <= 0)
            return 0;

        if (died)
            Die();

        Debug.Log($"{OwnerId}:U{UnitId}({unitType.UnitName}) took {appliedDamage} damage, current health: {health.CurrentHealth}");

        return appliedDamage;
    }

    private void Die()
    {
        int refund = passive.GetDeathRefund(unitType.Cost);

        if (refund > 0)
        {
            OnDeathRefund?.Invoke(refund);
            Presentation.ShowFeedback(passive.Type, refund);
        }

        HandleReward();
        Targeting.HandleOwnerDeath();

        Destroy(gameObject);
    }

    private void HandleReward()
    {
        BaseBehaviour rewardedBase = GameManager.Instance.GetRewardedBase(OwnerId);
        if (rewardedBase == null) return;

        RewardResult reward = rewardedBase.ClaimReward(unitType.Role, unitType.Cost);

        Presentation.ShowReward(reward, transform.right, rewardedBase.NeonMaterial);
    }

    void IAttackableObserver.RegisterObserver(IAttackableObserver observer) => Targeting.RegisterObserver(observer);
    void IAttackableObserver.UnregisterObserver(IAttackableObserver observer) => Targeting.UnregisterObserver(observer);
    void IAttackableObserver.HandleObservedTargetDestroyed(IAttackable destroyedTarget) => Targeting.HandleObservedTargetDestroyed(destroyedTarget);

    public void MirrorVisual() => Presentation.MirrorVisual();
}