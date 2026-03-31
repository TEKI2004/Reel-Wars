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

        ownerUnit.HandleBlockEnter(attackable);
    }

    private void OnTriggerExit(Collider other)
    {
        if (ownerUnit == null) return;

        IAttackable attackable = other.GetComponent<IAttackable>();
        if (attackable == null) return;
        if (other.transform.root == ownerUnit.transform.root) return;

        ownerUnit.HandleBlockExit(attackable);
    }
}
