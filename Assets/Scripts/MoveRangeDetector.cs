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
        if (ownerUnit == null) return;

        IAttackable attackable = other.GetComponent<IAttackable>();
        if (attackable == null) return;
        if (other.transform.root == ownerUnit.transform.root) return;

        var baseBehaviour = other.GetComponent<BaseBehaviour>();
        if (baseBehaviour != null && baseBehaviour.OwnerId == ownerUnit.OwnerId) return;

        ownerUnit.HandleBlockEnter(attackable);
    }

    private void OnTriggerExit(Collider other)
    {
        if (ownerUnit == null) return;

        IAttackable attackable = other.GetComponent<IAttackable>();
        if (attackable == null) return;
        if (other.transform.root == ownerUnit.transform.root) return;

        var baseBehaviour = other.GetComponent<BaseBehaviour>();
        if (baseBehaviour != null && baseBehaviour.OwnerId == ownerUnit.OwnerId) return;

        ownerUnit.HandleBlockExit(attackable);
    }
}
