using System.Collections.Generic;

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

        int maxTier = config.Stats?.Popularity?.MaxTier ?? defaultConfig.Popularity.MaxTier;
        DefaultAssetRegistry registry = DefaultAssetRegistry.Build(defaultConfig.StartingGenre, maxTier);

        errors.AddRange(registry.Errors);

        ValidateSchema(config, errors);
        ValidateStats(config.Stats, errors);
        ValidateUnits(config.Units, registry, errors);
        ValidateGenres(config.Genres, registry, errors);

        string startingGenreId = config.Stats?.StartingGenreId;

        if (!string.IsNullOrWhiteSpace(startingGenreId) && !registry.Genres.ContainsKey(startingGenreId))
            errors.Add($"Unknown starting genre ID '{startingGenreId}'.");

        return errors.Count == 0;
    }

    private static void ValidateSchema(ConfigData config, List<string> errors)
    {
        if (config.SchemaVersion.HasValue && config.SchemaVersion.Value != CurrentSchemaVersion)
            errors.Add($"Unsupported schema version '{config.SchemaVersion}'. Expected '{CurrentSchemaVersion}'.");
    }

    private static void ValidateStats(StatsConfigData stats, List<string> errors)
    {
        if (stats == null)
            return;

        ValidateMin(stats.BaseHealth, 1, "Stats.BaseHealth must be greater than 0.", errors);
        ValidateMin(stats.StartingMoney, 0, "Stats.StartingMoney cannot be negative.", errors);

        ValidateRewards(stats.Rewards, errors);
        ValidateSpamPunishment(stats.SpamPunishment, errors);
        ValidatePopularity(stats.Popularity, errors);
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

    private static void ValidatePopularity(PopularityConfigData popularity, List<string> errors)
    {
        if (popularity == null)
            return;

        ValidateMin(popularity.StartingPoints, 0, "Stats.Popularity.StartingPoints cannot be negative.", errors);
        ValidateMin(popularity.MaxTier, 1, "Stats.Popularity.MaxTier must be at least 1.", errors);
        ValidateMin(popularity.BaseLimit, 0, "Stats.Popularity.BaseLimit cannot be negative.", errors);
        ValidateMin(popularity.LinearLimitGrowth, 0, "Stats.Popularity.LinearLimitGrowth cannot be negative.", errors);
        ValidateMin(popularity.QuadraticLimitGrowth, 0, "Stats.Popularity.QuadraticLimitGrowth cannot be negative.", errors);
    }

    private static void ValidateOscarSeason(OscarSeasonConfigData oscarSeason, List<string> errors)
    {
        if (oscarSeason == null)
            return;

        ValidateMin(oscarSeason.Duration, float.Epsilon, "Stats.OscarSeason.Duration must be greater than 0.", errors);
        ValidateMin(oscarSeason.MoveSpeedMultiplier, 1f, "Stats.OscarSeason.MoveSpeedMultiplier must be at least 1.", errors);
        ValidateMin(oscarSeason.AttackSpeedMultiplier, 1f, "Stats.OscarSeason.AttackSpeedMultiplier must be at least 1.", errors);
    }

    private static void ValidateUnits(List<UnitConfigData> units, DefaultAssetRegistry registry, List<string> errors)
    {
        if (units == null)
            return;

        HashSet<string> ids = new();

        foreach (UnitConfigData unit in units)
        {
            if (unit == null)
            {
                errors.Add("Units cannot contain null entries.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(unit.Id))
            {
                errors.Add("Every unit override must have an ID.");
                continue;
            }

            if (!ids.Add(unit.Id))
                errors.Add($"Duplicate unit ID '{unit.Id}' in config.");

            if (!registry.Units.ContainsKey(unit.Id))
                errors.Add($"Unknown unit ID '{unit.Id}'.");

            ValidateMin(unit.Cost, 0, $"Unit '{unit.Id}': Cost cannot be negative.", errors);
            ValidateMin(unit.Damage, 0, $"Unit '{unit.Id}': Damage cannot be negative.", errors);
            ValidateMin(unit.MaxHealth, 1, $"Unit '{unit.Id}': MaxHealth must be greater than 0.", errors);
            ValidateMin(unit.AttackRange, float.Epsilon, $"Unit '{unit.Id}': AttackRange must be greater than 0.", errors);
            ValidateMin(unit.AttackCooldown, float.Epsilon, $"Unit '{unit.Id}': AttackCooldown must be greater than 0.", errors);
            ValidateMin(unit.MoveSpeed, float.Epsilon, $"Unit '{unit.Id}': MoveSpeed must be greater than 0.", errors);
        }
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

    private static void ValidateGenres(List<GenreConfigData> genres, DefaultAssetRegistry registry, List<string> errors)
    {
        if (genres == null)
            return;

        HashSet<string> ids = new();

        foreach (GenreConfigData genre in genres)
        {
            if (genre == null)
            {
                errors.Add("Genres cannot contain null entries.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(genre.Id))
            {
                errors.Add("Every genre override must have an ID.");
                continue;
            }

            if (!ids.Add(genre.Id))
                errors.Add($"Duplicate genre ID '{genre.Id}' in config.");

            if (!registry.Genres.ContainsKey(genre.Id))
            {
                errors.Add($"Unknown genre ID '{genre.Id}'.");
                continue;
            }

            ValidateGenreUnitReference(genre.Id, genre.MeleeUnitId, UnitRole.Melee, registry, errors);
            ValidateGenreUnitReference(genre.Id, genre.RangedUnitId, UnitRole.Ranged, registry, errors);
            ValidateGenreUnitReference(genre.Id, genre.HeavyUnitId, UnitRole.Heavy, registry, errors);

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

                if (!registry.Genres.ContainsKey(childId))
                    errors.Add($"Genre '{genre.Id}': Unknown child genre ID '{childId}'.");
            }
        }
    }

    private static void ValidateGenreUnitReference(
        string genreId,
        string unitId,
        UnitRole expectedRole,
        DefaultAssetRegistry registry,
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

        if (!registry.Units.TryGetValue(unitId, out UnitType unit))
        {
            errors.Add($"Genre '{genreId}': Unknown unit ID '{unitId}'.");
            return;
        }

        if (unit.Role != expectedRole)
            errors.Add($"Genre '{genreId}': Unit '{unitId}' has role '{unit.Role}', expected '{expectedRole}'.");
    }
}