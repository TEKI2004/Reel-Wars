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
        if (!TryGetEnemy(other, out IAttackable enemy))
            return;

        ownerUnit.Targeting.HandleEnemyEnter(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!TryGetEnemy(other, out IAttackable enemy))
            return;

        ownerUnit.Targeting.HandleEnemyExit(enemy);
    }

    private bool TryGetEnemy(Collider other, out IAttackable enemy)
    {
        enemy = null;

        if (ownerUnit == null || other.transform.root == ownerUnit.transform.root)
            return false;

        enemy = other.GetComponent<IAttackable>();

        return enemy != null && enemy.OwnerId != ownerUnit.OwnerId;
    }
}