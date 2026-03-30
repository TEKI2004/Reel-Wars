using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MoveRangeDetecor : MonoBehaviour
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

        ownerUnit.AddBlockingEntityToRange(attackable);
    }

    private void OnTriggerExit(Collider other)
    {
        if (ownerUnit == null) return;

        IAttackable attackable = other.GetComponent<IAttackable>();
        if (attackable == null) return;

        if (other.transform.root == ownerUnit.transform.root) return;

        ownerUnit.RemoveBlockingEntityFromRange(attackable);
    }
}
