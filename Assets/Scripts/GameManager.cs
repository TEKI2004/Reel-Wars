using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private Transform rightSpawnPoint;

    [Header("Unit Prefab")]
    [SerializeField] private GameObject unitPrefab;

    private void Start()
    {
        SpawnUnit(leftSpawnPoint, 0, UnitDirection.Right);
        SpawnUnit(rightSpawnPoint, 1, UnitDirection.Left);
    }

    private void SpawnUnit(Transform spawnPoint, int ownerId, UnitDirection direction)
    {
        if (unitPrefab == null || spawnPoint == null) return;

        GameObject unitObject = Instantiate(unitPrefab, spawnPoint.position, Quaternion.identity);
        UnitBehaviour unit = unitObject.GetComponent<UnitBehaviour>();

        if (unit != null)
        {
            unit.Initialize(ownerId, direction);
        }
    }
}