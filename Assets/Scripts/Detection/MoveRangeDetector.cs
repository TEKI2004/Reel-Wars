using UnityEngine;

public class MoveRangeDetector : MonoBehaviour
{
    [SerializeField] private UnitBehaviour ownerUnit;

    private void Reset()
    {
        ownerUnit = GetComponentInParent<UnitBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!TryGetBlockingEntity(other, out IAttackable entity))
            return;

        ownerUnit.Targeting.HandleBlockEnter(entity);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!TryGetBlockingEntity(other, out IAttackable entity))
            return;

        ownerUnit.Targeting.HandleBlockExit(entity);
    }

    private bool TryGetBlockingEntity(Collider other, out IAttackable entity)
    {
        entity = null;

        if (ownerUnit == null || other.transform.root == ownerUnit.transform.root)
            return false;

        entity = other.GetComponent<IAttackable>();

        if (entity == null)
            return false;

        return entity is not BaseBehaviour playerBase || playerBase.OwnerId != ownerUnit.OwnerId;
    }
}
