using UnityEngine;

public class AttackRangeDetector : MonoBehaviour
{
    [SerializeField] private UnitBehaviour ownerUnit;

    private void Reset()
    {
        ownerUnit = GetComponentInParent<UnitBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        IAttackable attackable = other.GetComponent<IAttackable>();

        if (attackable == null) return;
        if (attackable == (IAttackable)ownerUnit) return;
        if (attackable.OwnerId == ownerUnit.OwnerId) return;

        ownerUnit.AddEnemyToRange(attackable);
    }

    private void OnTriggerExit(Collider other)
    {
        IAttackable attackable = other.GetComponent<IAttackable>();

        if (attackable == null) return;

        ownerUnit.RemoveEnemyFromRange(attackable);
    }
}
