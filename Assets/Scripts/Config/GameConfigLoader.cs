using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public enum GameConfigLoadResult
{
    Missing,
    Success,
    Failed
}

public static class GameConfigLoader
{
    private const string ConfigDirectoryName = "Config";
    private const string ConfigFileName = "game-config.json";

    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Error
    };

    public static string ConfigPath => Path.GetFullPath(Path.Combine(Application.dataPath, "..", ConfigDirectoryName, ConfigFileName));

    public static GameConfigLoadResult Load(out GameConfigData config, out string error)
    {
        config = null;
        error = null;

        if (!File.Exists(ConfigPath))
            return GameConfigLoadResult.Missing;

        try
        {
            string json = File.ReadAllText(ConfigPath);
            config = JsonConvert.DeserializeObject<GameConfigData>(json, SerializerSettings);

            if (config == null)
            {
                error = $"Config file contains no configuration data: {ConfigPath}";
                return GameConfigLoadResult.Failed;
            }

            return GameConfigLoadResult.Success;
        }
        catch (JsonException exception)
        {
            error = $"Failed to parse game config '{ConfigPath}':\n{exception.Message}";
            return GameConfigLoadResult.Failed;
        }
        catch (Exception exception)
        {
            error = $"Failed to read game config '{ConfigPath}':\n{exception.Message}";
            return GameConfigLoadResult.Failed;
        }
    }
}