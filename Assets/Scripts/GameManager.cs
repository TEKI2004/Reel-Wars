using Unity.ProjectAuditor.Editor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private Transform rightSpawnPoint;

    [Header("Spawn Area")]
    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private BoxCollider leftSpawnArea;
    [SerializeField] private BoxCollider rightSpawnArea;

    [Header("Unit Prefab")]
    [SerializeField] private GameObject unitPrefab;

    [Header("Names")]
    [SerializeField] private string leftPlayerName = "Player1";
    [SerializeField] private string rightPlayerName = "Player2";

    public string LeftPlayerName => leftPlayerName;
    public string RightPlayerName => rightPlayerName;


    private void Start()
    {
        //SpawnUnit(FacingDirection.Right);
        //SpawnUnit(FacingDirection.Left);
    }

    public void SpawnUnit(FacingDirection direction)
    {
        if (unitPrefab == null) return;

        string ownerId = GetOwnerId(direction);

        if (!IsSpawnAreaFree(ownerId, direction))
        {
            Debug.LogWarning("Spawn area is occupied. Cannot spawn unit.");
            return;
        }

        Transform spawnPoint = GetSpawnPoint(direction);
        GameObject unitObject = Instantiate(unitPrefab, spawnPoint.position, Quaternion.identity);
        UnitBehaviour unit = unitObject.GetComponent<UnitBehaviour>();

        if (unit != null)
        {
            unit.Initialize(ownerId, direction);
        }
    }

    private Transform GetSpawnPoint(FacingDirection direction)
    {
        return direction == FacingDirection.Left ? rightSpawnPoint : leftSpawnPoint;
    }

    private BoxCollider GetSpawnArea(FacingDirection direction)
    {
        return direction == FacingDirection.Left ? rightSpawnArea : leftSpawnArea;
    }

    private string GetOwnerId(FacingDirection direction)
    {
        return direction == FacingDirection.Left ? RightPlayerName : LeftPlayerName;
    }

    private bool IsSpawnAreaFree(string ownerId, FacingDirection direction)
    {
        BoxCollider spawnArea = GetSpawnArea(direction);

        Collider[] hits = Physics.OverlapBox(
            spawnArea.bounds.center,
            spawnArea.bounds.extents,
            Quaternion.identity,
            unitLayer
        );

        foreach (var hit in hits)
        {
            var attackable = hit.GetComponent<IAttackable>();
            if (attackable != null && attackable.OwnerId == ownerId)
            {
                return false;
            }
        }

        return true;
    }

}