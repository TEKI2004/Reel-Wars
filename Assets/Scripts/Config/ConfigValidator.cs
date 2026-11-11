using System.Collections.Generic;
using System.IO;

public static class ConfigValidator
{
    private const int CurrentSchemaVersion = 1;

    public static bool Validate(ConfigData config, StatsConfig defaultConfig, out List<string> errors)
    {
        errors = new List<string>();

        if (defaultConfig == null)
        {
            errors.Add("Default StatsConfig is missing.");
            return false;
        }

        if (defaultConfig.StartingGenre == null)
        {
            errors.Add("Default StartingGenre is missing.");
            return false;
        }

        DefaultAssetRegistry registry = DefaultAssetRegistry.Build(
            defaultConfig.StartingGenre,
            defaultConfig.Popularity.MaxTier
        );

        errors.AddRange(registry.Errors);

        Dictionary<string, UnitRole?> availableUnits = new();

        foreach (KeyValuePair<string, UnitType> pair in registry.Units)
            availableUnits.Add(pair.Key, pair.Value.Role);

        HashSet<string> availableGenres = new(registry.Genres.Keys);

        if (config.Genres != null)
        {
            foreach (GenreConfigData genre in config.Genres)
            {
                if (genre != null && !string.IsNullOrWhiteSpace(genre.Id))
                    availableGenres.Add(genre.Id);
            }
        }

        ValidateSchema(config, errors);
        ValidateStats(config.Stats, defaultConfig, errors);
        ValidateUnits(config.Units, registry, availableUnits, errors);
        ValidateGenres(config.Genres, registry, availableUnits, availableGenres, errors);

        string startingGenreId = config.Stats?.StartingGenreId;

        if (!string.IsNullOrWhiteSpace(startingGenreId) && !availableGenres.Contains(startingGenreId))
            errors.Add($"Unknown starting genre ID '{startingGenreId}'.");

        return errors.Count == 0;
    }

    private static void ValidateSchema(ConfigData config, List<string> errors)
    {
        if (config.SchemaVersion.HasValue && config.SchemaVersion.Value != CurrentSchemaVersion)
            errors.Add($"Unsupported schema version '{config.SchemaVersion}'. Expected '{CurrentSchemaVersion}'.");
    }

    private static void ValidateStats(StatsConfigData stats, StatsConfig defaults, List<string> errors)
    {
        if (stats == null)
            return;

        ValidateMin(stats.BaseHealth, 1, "Stats.BaseHealth must be greater than 0.", errors);
        ValidateMin(stats.StartingMoney, 0, "Stats.StartingMoney cannot be negative.", errors);

        ValidateRewards(stats.Rewards, errors);
        ValidateSpamPunishment(stats.SpamPunishment, errors);
        ValidatePopularity(stats.Popularity, defaults.Popularity, errors);
        ValidateOscarSeason(stats.OscarSeason, errors);
    }

    private static void ValidateRewards(RewardConfigData rewards, List<string> errors)
    {
        if (rewards == null)
            return;

        ValidateMin(rewards.MeleeDeathPoints, 0, "Stats.Rewards.MeleeDeathPoints cannot be negative.", errors);
        ValidateMin(rewards.RangedDeathPoints, 0, "Stats.Rewards.RangedDeathPoints cannot be negative.", errors);
        ValidateMin(rewards.HeavyDeathPoints, 0, "Stats.Rewards.HeavyDeathPoints cannot be negative.", errors);
        ValidateMin(rewards.CostMultiplier, 0f, "Stats.Rewards.CostMultiplier cannot be negative.", errors);
    }

    private static void ValidateSpamPunishment(SpamPunishmentConfigData spamPunishment, List<string> errors)
    {
        if (spamPunishment == null)
            return;

        ValidateMin(spamPunishment.PenaltyPerTier, 0, "Stats.SpamPunishment.PenaltyPerTier cannot be negative.", errors);
    }

    private static void ValidatePopularity(
        PopularityConfigData overrides,
        PopularityConfig defaults,
        List<string> errors
    )
    {
        if (overrides == null)
            return;

        ValidateMin(overrides.StartingPoints, 0, "Stats.Popularity.StartingPoints cannot be negative.", errors);
        ValidateMin(overrides.MaxTier, 1, "Stats.Popularity.MaxTier must be at least 1.", errors);
        ValidateMin(overrides.BaseLimit, 0, "Stats.Popularity.BaseLimit cannot be negative.", errors);
        ValidateMin(overrides.LinearLimitGrowth, 0, "Stats.Popularity.LinearLimitGrowth cannot be negative.", errors);
        ValidateMin(overrides.QuadraticLimitGrowth, 0, "Stats.Popularity.QuadraticLimitGrowth cannot be negative.", errors);

        int startingPoints = overrides.StartingPoints ?? defaults.StartingPoints;
        int maxTier = overrides.MaxTier ?? defaults.MaxTier;
        int baseLimit = overrides.BaseLimit ?? defaults.BaseLimit;
        int linearGrowth = overrides.LinearLimitGrowth ?? defaults.LinearLimitGrowth;
        int quadraticGrowth = overrides.QuadraticLimitGrowth ?? defaults.QuadraticLimitGrowth;

        int maxPoints =
            baseLimit
            + maxTier * linearGrowth
            + maxTier * maxTier * quadraticGrowth;

        if (startingPoints > maxPoints)
            errors.Add($"Stats.Popularity.StartingPoints ({startingPoints}) cannot exceed MaxPoints ({maxPoints}).");
    }

    private static void ValidateOscarSeason(OscarSeasonConfigData oscarSeason, List<string> errors)
    {
        if (oscarSeason == null)
            return;

        ValidateMin(oscarSeason.Duration, float.Epsilon, "Stats.OscarSeason.Duration must be greater than 0.", errors);
        ValidateMin(oscarSeason.MoveSpeedMultiplier, 1f, "Stats.OscarSeason.MoveSpeedMultiplier must be at least 1.", errors);
        ValidateMin(oscarSeason.AttackSpeedMultiplier, 1f, "Stats.OscarSeason.AttackSpeedMultiplier must be at least 1.", errors);
    }

    private static void ValidateUnits(
        List<UnitConfigData> units,
        DefaultAssetRegistry registry,
        Dictionary<string, UnitRole?> availableUnits,
        List<string> errors
    )
    {
        if (units == null)
            return;

        HashSet<string> configIds = new();

        foreach (UnitConfigData unit in units)
        {
            if (unit == null)
            {
                errors.Add("Units cannot contain null entries.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(unit.Id))
            {
                errors.Add("Every unit must have an ID.");
                continue;
            }

            if (!configIds.Add(unit.Id))
            {
                errors.Add($"Duplicate unit ID '{unit.Id}' in config.");
                continue;
            }

            bool isNewUnit = !registry.Units.TryGetValue(unit.Id, out UnitType defaultUnit);

            if (isNewUnit)
            {
                ValidateNewUnitDefinition(unit, errors);
                availableUnits[unit.Id] = unit.Role;
            }
            else
            {
                if (unit.Name != null && string.IsNullOrWhiteSpace(unit.Name))
                    errors.Add($"Unit '{unit.Id}': Name cannot be empty.");

                if (unit.Role.HasValue)
                    availableUnits[unit.Id] = unit.Role.Value;
            }

            ValidateMin(unit.Cost, 0, $"Unit '{unit.Id}': Cost cannot be negative.", errors);
            ValidateMin(unit.Damage, 0, $"Unit '{unit.Id}': Damage cannot be negative.", errors);
            ValidateMin(unit.MaxHealth, 1, $"Unit '{unit.Id}': MaxHealth must be greater than 0.", errors);
            ValidateMin(unit.AttackRange, float.Epsilon, $"Unit '{unit.Id}': AttackRange must be greater than 0.", errors);
            ValidateMin(unit.AttackCooldown, float.Epsilon, $"Unit '{unit.Id}': AttackCooldown must be greater than 0.", errors);
            ValidateMin(unit.MoveSpeed, float.Epsilon, $"Unit '{unit.Id}': MoveSpeed must be greater than 0.", errors);
        }
    }

    private static void ValidateNewUnitDefinition(UnitConfigData unit, List<string> errors)
    {
        string prefix = $"New unit '{unit.Id}'";

        if (string.IsNullOrWhiteSpace(unit.Name))
            errors.Add($"{prefix}: Name is required.");

        if (!unit.Role.HasValue)
            errors.Add($"{prefix}: Role is required.");

        if (!unit.Cost.HasValue)
            errors.Add($"{prefix}: Cost is required.");

        if (!unit.Damage.HasValue)
            errors.Add($"{prefix}: Damage is required.");

        if (!unit.MaxHealth.HasValue)
            errors.Add($"{prefix}: MaxHealth is required.");

        if (!unit.AttackRange.HasValue)
            errors.Add($"{prefix}: AttackRange is required.");

        if (!unit.AttackCooldown.HasValue)
            errors.Add($"{prefix}: AttackCooldown is required.");

        if (!unit.MoveSpeed.HasValue)
            errors.Add($"{prefix}: MoveSpeed is required.");

        ValidateRequiredFile(ConfigLoader.GetModelPath(unit.Id), $"{prefix}: GLB model", errors);
        ValidateRequiredFile(ConfigLoader.GetUnitIconPath(unit.Id), $"{prefix}: Icon", errors);
    }

    private static void ValidateGenres(
        List<GenreConfigData> genres,
        DefaultAssetRegistry registry,
        Dictionary<string, UnitRole?> availableUnits,
        HashSet<string> availableGenres,
        List<string> errors
    )
    {
        if (genres == null)
            return;

        HashSet<string> configIds = new();

        foreach (GenreConfigData genre in genres)
        {
            if (genre == null)
            {
                errors.Add("Genres cannot contain null entries.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(genre.Id))
            {
                errors.Add("Every genre must have an ID.");
                continue;
            }

            if (!configIds.Add(genre.Id))
            {
                errors.Add($"Duplicate genre ID '{genre.Id}' in config.");
                continue;
            }

            bool isNewGenre = !registry.Genres.ContainsKey(genre.Id);

            if (isNewGenre)
            {
                ValidateNewGenreDefinition(genre, errors);
            }
            else if (genre.Name != null && string.IsNullOrWhiteSpace(genre.Name))
            {
                errors.Add($"Genre '{genre.Id}': Name cannot be empty.");
            }

            ValidateGenreUnitReference(genre.Id, genre.MeleeUnitId, UnitRole.Melee, availableUnits, errors);
            ValidateGenreUnitReference(genre.Id, genre.RangedUnitId, UnitRole.Ranged, availableUnits, errors);
            ValidateGenreUnitReference(genre.Id, genre.HeavyUnitId, UnitRole.Heavy, availableUnits, errors);

            if (genre.Children == null)
                continue;

            if (genre.Children.Count != 0 && genre.Children.Count != 2)
                errors.Add($"Genre '{genre.Id}' must have either 0 or 2 children.");

            foreach (string childId in genre.Children)
            {
                if (string.IsNullOrWhiteSpace(childId))
                {
                    errors.Add($"Genre '{genre.Id}' contains an empty child ID.");
                    continue;
                }

                if (!availableGenres.Contains(childId))
                    errors.Add($"Genre '{genre.Id}': Unknown child genre ID '{childId}'.");
            }
        }
    }

    private static void ValidateNewGenreDefinition(GenreConfigData genre, List<string> errors)
    {
        string prefix = $"New genre '{genre.Id}'";

        if (string.IsNullOrWhiteSpace(genre.Name))
            errors.Add($"{prefix}: Name is required.");

        if (string.IsNullOrWhiteSpace(genre.MeleeUnitId))
            errors.Add($"{prefix}: MeleeUnitId is required.");

        if (string.IsNullOrWhiteSpace(genre.RangedUnitId))
            errors.Add($"{prefix}: RangedUnitId is required.");

        if (string.IsNullOrWhiteSpace(genre.HeavyUnitId))
            errors.Add($"{prefix}: HeavyUnitId is required.");

        if (genre.Children == null)
            errors.Add($"{prefix}: Children is required. Use an empty array for a leaf genre.");

        ValidateRequiredFile(ConfigLoader.GetGenreIconPath(genre.Id), $"{prefix}: Icon", errors);
    }

    private static void ValidateGenreUnitReference(
        string genreId,
        string unitId,
        UnitRole expectedRole,
        Dictionary<string, UnitRole?> availableUnits,
        List<string> errors
    )
    {
        if (unitId == null)
            return;

        if (string.IsNullOrWhiteSpace(unitId))
        {
            errors.Add($"Genre '{genreId}': {expectedRole} unit ID cannot be empty.");
            return;
        }

        if (!availableUnits.TryGetValue(unitId, out UnitRole? actualRole))
        {
            errors.Add($"Genre '{genreId}': Unknown unit ID '{unitId}'.");
            return; 
        }

        if (!actualRole.HasValue)
            return; // Missing role is already reported when validating the new unit.

        if (actualRole.Value != expectedRole)
            errors.Add($"Genre '{genreId}': Unit '{unitId}' has role '{actualRole.Value}', expected '{expectedRole}'.");
    }

    private static void ValidateMin(int? value, int min, string message, List<string> errors)
    {
        if (value.HasValue && value.Value < min)
            errors.Add(message);
    }

    private static void ValidateMin(float? value, float min, string message, List<string> errors)
    {
        if (value.HasValue && value.Value < min)
            errors.Add(message);
    }
    private static void ValidateRequiredFile(string path, string description, List<string> errors)
    {
        if (!File.Exists(path))
            errors.Add($"{description} not found: {path}");
    }
}