using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private Transform rightSpawnPoint;

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

        Transform spawnPoint;
        string ownerId;
        if (direction == FacingDirection.Left)
        {
            spawnPoint = rightSpawnPoint;
            ownerId = RightPlayerName;
        }
        else
        {
            spawnPoint = leftSpawnPoint;
            ownerId = LeftPlayerName;
        }

        GameObject unitObject = Instantiate(unitPrefab, spawnPoint.position, Quaternion.identity);
        UnitBehaviour unit = unitObject.GetComponent<UnitBehaviour>();

        if (unit != null)
        {
            unit.Initialize(ownerId, direction);
        }
    }
}