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

    private RuntimeConfig runtimeConfig;
    public StatsConfig Config => runtimeConfig?.Stats;

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
            defaultStatsConfig = Resources.Load<StatsConfig>("StatsConfig");

        if (defaultStatsConfig == null)
        {
            Debug.LogError("Default StatsConfig could not be found.");
            enabled = false;
            return;
        }

        BuildRuntimeConfig();

        if (leftPlayerName == rightPlayerName)
            Debug.LogError("Player names must be unique.");

        leftBase.Initialize(leftPlayerName, FacingDirection.Right);
        rightBase.Initialize(rightPlayerName, FacingDirection.Left);
    }

    private void Start()
    {
        leftBase.Player.Popularity.CheckTierProgression();
        rightBase.Player.Popularity.CheckTierProgression();
    }

    private void BuildRuntimeConfig()
    {
        ConfigLoadResult loadResult = ConfigLoader.Load(out ConfigData configData, out string error);

        if (loadResult == ConfigLoadResult.Missing)
        {
            Debug.Log($"No external game config found. Using defaults.\n{ConfigLoader.ConfigPath}");
            runtimeConfig = ConfigBuilder.Build(defaultStatsConfig, null);
            return;
        }

        if (loadResult == ConfigLoadResult.Failed)
        {
            Debug.LogError($"{error}\nFalling back to default configuration.");
            runtimeConfig = ConfigBuilder.Build(defaultStatsConfig, null);
            return;
        }

        if (!ConfigValidator.Validate(configData, defaultStatsConfig, out var errors))
        {
            Debug.LogError($"Game config validation failed:\n- {string.Join("\n- ", errors)}\nFalling back to default configuration.");
            runtimeConfig = ConfigBuilder.Build(defaultStatsConfig, null);
            return;
        }

        runtimeConfig = ConfigBuilder.Build(defaultStatsConfig, configData);

        Debug.Log($"External game config loaded successfully.\n{ConfigLoader.ConfigPath}");
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