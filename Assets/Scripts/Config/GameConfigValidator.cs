using System.Collections.Generic;

public static class GameConfigValidator
{
    private const int CurrentSchemaVersion = 1;

    public static bool Validate(GameConfigData config, StatsConfig defaultConfig, out List<string> errors)
    {
        errors = new List<string>();

        ValidateSchema(config, errors);
        ValidateStats(config.Stats, errors);
        ValidateUnits(config.Units, errors);
        ValidateReferences(config, defaultConfig, errors);

        return errors.Count == 0;
    }

    private static void ValidateSchema(GameConfigData config, List<string> errors)
    {
        if (config.SchemaVersion.HasValue && config.SchemaVersion.Value != CurrentSchemaVersion)
            errors.Add($"Unsupported schema version '{config.SchemaVersion}'. Expected '{CurrentSchemaVersion}'.");
    }

    private static void ValidateStats(StatsConfigData stats, List<string> errors)
    {
        if (stats == null)
            return;

        if (stats.BaseHealth.HasValue && stats.BaseHealth <= 0)
            errors.Add("Stats.BaseHealth must be greater than 0.");

        if (stats.StartingMoney.HasValue && stats.StartingMoney < 0)
            errors.Add("Stats.StartingMoney cannot be negative.");

        ValidateRewards(stats.Rewards, errors);
        ValidateSpamPunishment(stats.SpamPunishment, errors);
        ValidatePopularity(stats.Popularity, errors);
        ValidateOscarSeason(stats.OscarSeason, errors);
    }

    private static void ValidateRewards(RewardConfigData rewards, List<string> errors)
    {
        if (rewards == null)
            return;

        if (rewards.MeleeDeathPoints.HasValue && rewards.MeleeDeathPoints < 0)
            errors.Add("Stats.Rewards.MeleeDeathPoints cannot be negative.");

        if (rewards.RangedDeathPoints.HasValue && rewards.RangedDeathPoints < 0)
            errors.Add("Stats.Rewards.RangedDeathPoints cannot be negative.");

        if (rewards.HeavyDeathPoints.HasValue && rewards.HeavyDeathPoints < 0)
            errors.Add("Stats.Rewards.HeavyDeathPoints cannot be negative.");

        if (rewards.CostMultiplier.HasValue && rewards.CostMultiplier < 0f)
            errors.Add("Stats.Rewards.CostMultiplier cannot be negative.");
    }

    private static void ValidateSpamPunishment(SpamPunishmentConfigData spamPunishment, List<string> errors)
    {
        if (spamPunishment == null)
            return;

        if (spamPunishment.PenaltyPerTier.HasValue && spamPunishment.PenaltyPerTier < 0)
            errors.Add("Stats.SpamPunishment.PenaltyPerTier cannot be negative.");
    }

    private static void ValidatePopularity(PopularityConfigData popularity, List<string> errors)
    {
        if (popularity == null)
            return;

        if (popularity.StartingPoints.HasValue && popularity.StartingPoints < 0)
            errors.Add("Stats.Popularity.StartingPoints cannot be negative.");

        if (popularity.MaxTier.HasValue && popularity.MaxTier < 1)
            errors.Add("Stats.Popularity.MaxTier must be at least 1.");

        if (popularity.BaseLimit.HasValue && popularity.BaseLimit < 0)
            errors.Add("Stats.Popularity.BaseLimit cannot be negative.");

        if (popularity.LinearLimitGrowth.HasValue && popularity.LinearLimitGrowth < 0)
            errors.Add("Stats.Popularity.LinearLimitGrowth cannot be negative.");

        if (popularity.QuadraticLimitGrowth.HasValue && popularity.QuadraticLimitGrowth < 0)
            errors.Add("Stats.Popularity.QuadraticLimitGrowth cannot be negative.");
    }

    private static void ValidateOscarSeason(OscarSeasonConfigData oscarSeason, List<string> errors)
    {
        if (oscarSeason == null)
            return;

        if (oscarSeason.Duration.HasValue && oscarSeason.Duration <= 0f)
            errors.Add("Stats.OscarSeason.Duration must be greater than 0.");

        if (oscarSeason.MoveSpeedMultiplier.HasValue && oscarSeason.MoveSpeedMultiplier < 1f)
            errors.Add("Stats.OscarSeason.MoveSpeedMultiplier must be at least 1.");

        if (oscarSeason.AttackSpeedMultiplier.HasValue && oscarSeason.AttackSpeedMultiplier < 1f)
            errors.Add("Stats.OscarSeason.AttackSpeedMultiplier must be at least 1.");
    }

    private static void ValidateUnits(List<UnitConfigData> units, List<string> errors)
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

            ValidateUnit(unit, errors);
        }
    }

    private static void ValidateUnit(UnitConfigData unit, List<string> errors)
    {
        string prefix = $"Unit '{unit.Id}'";

        if (unit.Cost.HasValue && unit.Cost < 0)
            errors.Add($"{prefix}: Cost cannot be negative.");

        if (unit.Damage.HasValue && unit.Damage < 0)
            errors.Add($"{prefix}: Damage cannot be negative.");

        if (unit.MaxHealth.HasValue && unit.MaxHealth <= 0)
            errors.Add($"{prefix}: MaxHealth must be greater than 0.");

        if (unit.AttackRange.HasValue && unit.AttackRange < 0f)
            errors.Add($"{prefix}: AttackRange cannot be negative.");

        if (unit.AttackCooldown.HasValue && unit.AttackCooldown <= 0f)
            errors.Add($"{prefix}: AttackCooldown must be greater than 0.");

        if (unit.MoveSpeed.HasValue && unit.MoveSpeed < 0f)
            errors.Add($"{prefix}: MoveSpeed cannot be negative.");
    }

    private static void ValidateReferences(GameConfigData config, StatsConfig defaultConfig, List<string> errors)
    {
        if (defaultConfig == null)
        {
            errors.Add("Default StatsConfig is missing.");
            return;
        }

        if (defaultConfig.StartingGenre == null)
        {
            errors.Add("Default StartingGenre is missing.");
            return;
        }

        GameConfigAssetRegistry registry = GameConfigAssetRegistry.Build(defaultConfig.StartingGenre);

        ValidateAssetRegistry(registry, config, defaultConfig, errors);
        ValidateStartingGenre(config.Stats?.StartingGenreId, registry, errors);
        ValidateUnitReferences(config.Units, registry, errors);
    }

    private static void ValidateAssetRegistry(GameConfigAssetRegistry registry, GameConfigData config, StatsConfig defaultConfig, List<string> errors)
    {
        errors.AddRange(registry.Errors);

        int maxTier = config.Stats?.Popularity?.MaxTier ?? defaultConfig.Popularity.MaxTier;

        if (registry.MaxDepth > maxTier)
            errors.Add($"Genre tree depth ({registry.MaxDepth}) exceeds MaxTier ({maxTier}).");
    }

    private static void ValidateStartingGenre(string startingGenreId, GameConfigAssetRegistry registry, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(startingGenreId))
            return;

        if (!registry.Genres.ContainsKey(startingGenreId))
            errors.Add($"Unknown starting genre ID '{startingGenreId}'.");
    }

    private static void ValidateUnitReferences(List<UnitConfigData> unitOverrides, GameConfigAssetRegistry registry, List<string> errors)
    {
        if (unitOverrides == null)
            return;

        foreach (UnitConfigData unitOverride in unitOverrides)
        {
            if (unitOverride == null || string.IsNullOrWhiteSpace(unitOverride.Id))
                continue;

            if (!registry.Units.ContainsKey(unitOverride.Id))
                errors.Add($"Unknown unit ID '{unitOverride.Id}'.");
        }
    }
}