using System;
using System.Threading.Tasks;
using UnityEngine;

[DefaultExecutionOrder(-2000)]
public class GameManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private StatsConfig defaultStatsConfig;
    [SerializeField] private UnitBehaviour unitTemplate;
    [SerializeField] private GameObject gameRuntimeRoot;

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

        if (leftPlayerName == rightPlayerName)
        {
            Debug.LogError("Player names must be unique.");
            enabled = false;
            return;
        }

        gameRuntimeRoot.SetActive(false);
    }

    private async void Start()
    {
        try
        {
            runtimeConfig = await BuildRuntimeConfigAsync();

            leftBase.Initialize(leftPlayerName, FacingDirection.Right);
            rightBase.Initialize(rightPlayerName, FacingDirection.Left);

            gameRuntimeRoot.SetActive(true);

            leftBase.Player.Popularity.CheckTierProgression();
            rightBase.Player.Popularity.CheckTierProgression();
        }
        catch (Exception exception)
        {
            Debug.LogError("Game initialization failed.");
            Debug.LogException(exception);
            enabled = false;
        }
    }

    private async Task<RuntimeConfig> BuildRuntimeConfigAsync()
    {
        ConfigLoadResult loadResult = ConfigLoader.Load(out ConfigData configData, out string error);

        if (loadResult == ConfigLoadResult.Missing)
        {
            Debug.Log($"No external game config found. Using defaults.\n{ConfigLoader.ConfigPath}");
            return await ConfigBuilder.BuildAsync(defaultStatsConfig, null, unitTemplate);
        }

        if (loadResult == ConfigLoadResult.Failed)
        {
            Debug.LogError($"{error}\nFalling back to default configuration.");
            return await ConfigBuilder.BuildAsync(defaultStatsConfig, null, unitTemplate);
        }

        if (!ConfigValidator.Validate(configData, defaultStatsConfig, out var errors))
        {
            Debug.LogError($"Game config validation failed:\n- {string.Join("\n- ", errors)}\nFalling back to default configuration.");
            return await ConfigBuilder.BuildAsync(defaultStatsConfig, null, unitTemplate);
        }

        try
        {
            RuntimeConfig config = await ConfigBuilder.BuildAsync(defaultStatsConfig, configData, unitTemplate);

            Debug.Log($"External game config loaded successfully.\n{ConfigLoader.ConfigPath}");
            return config;
        }
        catch (Exception exception)
        {
            Debug.LogError("Failed to load external PNG/GLB assets. Falling back to default configuration.");
            Debug.LogException(exception);

            ExternalAssetLoader.ReleaseAll();

            return await ConfigBuilder.BuildAsync(defaultStatsConfig, null, unitTemplate);
        }
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

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        ExternalAssetLoader.ReleaseAll();
        Instance = null;
    }
}