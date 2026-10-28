using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public enum ConfigLoadResult
{
    Missing,
    Success,
    Failed
}

public static class ConfigLoader
{
    private const string ConfigDirectoryName = "Config";
    private const string ConfigFileName = "game-config.json";

    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Error
    };

    public static string ConfigPath => Path.GetFullPath(Path.Combine(Application.dataPath, "..", ConfigDirectoryName, ConfigFileName));

    public static ConfigLoadResult Load(out ConfigData config, out string error)
    {
        config = null;
        error = null;

        if (!File.Exists(ConfigPath))
            return ConfigLoadResult.Missing;

        try
        {
            string json = File.ReadAllText(ConfigPath);
            config = JsonConvert.DeserializeObject<ConfigData>(json, SerializerSettings);

            if (config == null)
            {
                error = $"Config file contains no configuration data: {ConfigPath}";
                return ConfigLoadResult.Failed;
            }

            return ConfigLoadResult.Success;
        }
        catch (JsonException exception)
        {
            error = $"Failed to parse game config '{ConfigPath}':\n{exception.Message}";
            return ConfigLoadResult.Failed;
        }
        catch (Exception exception)
        {
            error = $"Failed to read game config '{ConfigPath}':\n{exception.Message}";
            return ConfigLoadResult.Failed;
        }
    }
}