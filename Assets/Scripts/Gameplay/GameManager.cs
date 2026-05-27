using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    [Header("Config")]
    [SerializeField] private StatsConfig defaultStatsConfig;

    [Header("Names")]
    [SerializeField] private string leftPlayerName = "Player1";
    [SerializeField] private string rightPlayerName = "Player2";

    [Header("Base Behaviours")]
    [SerializeField] private BaseBehaviour leftBase;
    [SerializeField] private BaseBehaviour rightBase;

    public BaseBehaviour LeftBase => leftBase;
    public BaseBehaviour RightBase => rightBase;

    public StatsConfig Config => defaultStatsConfig;

    public static GameManager Instance { get; private set; } 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (defaultStatsConfig == null)
        { 
            defaultStatsConfig = Resources.Load<StatsConfig>("StatsConfig");
        }

        if (leftPlayerName == rightPlayerName)
        {
            Debug.LogError("Player names must be unique.");
        }

        leftBase.Initialize(leftPlayerName, FacingDirection.Right);
        rightBase.Initialize(rightPlayerName, FacingDirection.Left);
    }

    public void GiveReward(string victimOwnerId, UnitRole victimRole, int victimCost)
    {
        if (victimOwnerId == leftPlayerName)
        {
            rightBase.ClaimReward(victimRole, victimCost);
        }
        else if (victimOwnerId == rightPlayerName)
        {
            leftBase.ClaimReward(victimRole, victimCost);
        }
        else
        {
            Debug.LogError($"Invalid victim owner ID: {victimOwnerId}");
        }
    }

}