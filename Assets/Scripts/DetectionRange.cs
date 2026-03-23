using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DetectionRange : MonoBehaviour
{
    [SerializeField] private UnitBehaviour ownerUnit;

    private void OnTriggerEnter(Collider other)
    {
        IAttackable enemy = other.GetComponent<IAttackable>();
        if (enemy != null && enemy.OwnerId != ownerUnit.OwnerId)
        {
            ownerUnit.AddEnemyToRange(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IAttackable enemy = other.GetComponent<IAttackable>();
        if (enemy != null && ownerUnit != null)
        {
            ownerUnit.RemoveEnemyFromRange(enemy);
        }
    }
}
