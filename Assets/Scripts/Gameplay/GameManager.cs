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

    [Header("Oscar Season Controllers")]
    [SerializeField] private OscarSeasonController leftOscarSeason;
    [SerializeField] private OscarSeasonController rightOscarSeason;

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

    private void Start()
    {
        leftBase.Player.Popularity.CheckTierProgression();
        rightBase.Player.Popularity.CheckTierProgression();
    }

    public BaseBehaviour GetRewardedBase(string victimOwnerId)
    {
        if (victimOwnerId == leftBase.OwnerId) return rightBase;
        if (victimOwnerId == rightBase.OwnerId) return leftBase;

        Debug.LogError($"Invalid victim owner ID: {victimOwnerId}");
        return null;
    }

    public OscarSeasonController GetOscarSeason(string ownerId)
    {
        if (ownerId == leftBase.OwnerId) return leftOscarSeason;
        if (ownerId == rightBase.OwnerId) return rightOscarSeason;

        Debug.LogError($"Oscar Season controller not found for owner ID: {ownerId}");
        return null;
    }
}