using UnityEngine;

public class DetectionRange : MonoBehaviour
{
    [SerializeField] private UnitBehaviour ownerUnit;

    private void OnTriggerEnter(Collider other)
    {
        UnitBehaviour enemy = other.GetComponent<UnitBehaviour>();
        if (enemy != null)
        {
            ownerUnit.AddEnemyToRange(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        UnitBehaviour enemy = other.GetComponent<UnitBehaviour>();
        if (enemy != null)
        {
            ownerUnit.RemoveEnemyFromRange(enemy);
        }
    }
}
