using System.Collections.Generic;
using UnityEngine;

public static class GameConfigBuilder
{
    public static RuntimeGameConfig Build(StatsConfig defaultConfig, GameConfigData configData)
    {
        StatsConfig runtimeStats = Object.Instantiate(defaultConfig);
        runtimeStats.name = $"{defaultConfig.name} (Runtime)";

        CloneNestedConfigs(runtimeStats, defaultConfig);
        ApplyStatsOverrides(runtimeStats, configData?.Stats);

        GameConfigAssetRegistry registry = GameConfigAssetRegistry.Build(defaultConfig.StartingGenre);

        Dictionary<string, UnitType> runtimeUnits = BuildRuntimeUnits(registry.Units, configData?.Units);
        Dictionary<string, GenreNode> runtimeGenres = BuildRuntimeGenres(registry.Genres, runtimeUnits);

        string startingGenreId = configData?.Stats?.StartingGenreId ?? defaultConfig.StartingGenre.Id;
        runtimeStats.StartingGenre = runtimeGenres[startingGenreId];

        return new RuntimeGameConfig(runtimeStats, runtimeUnits, runtimeGenres);
    }

    private static void CloneNestedConfigs(StatsConfig runtimeConfig, StatsConfig defaultConfig)
    {
        runtimeConfig.Rewards = new RewardConfig
        {
            MeleeDeathPoints = defaultConfig.Rewards.MeleeDeathPoints,
            RangedDeathPoints = defaultConfig.Rewards.RangedDeathPoints,
            HeavyDeathPoints = defaultConfig.Rewards.HeavyDeathPoints,
            CostMultiplier = defaultConfig.Rewards.CostMultiplier
        };

        runtimeConfig.Popularity = new PopularityConfig
        {
            StartingPoints = defaultConfig.Popularity.StartingPoints,
            MaxTier = defaultConfig.Popularity.MaxTier,
            BaseLimit = defaultConfig.Popularity.BaseLimit,
            LinearLimitGrowth = defaultConfig.Popularity.LinearLimitGrowth,
            QuadraticLimitGrowth = defaultConfig.Popularity.QuadraticLimitGrowth
        };

        runtimeConfig.OscarSeason = new OscarSeasonConfig
        {
            Duration = defaultConfig.OscarSeason.Duration,
            MoveSpeedMultiplier = defaultConfig.OscarSeason.MoveSpeedMultiplier,
            AttackSpeedMultiplier = defaultConfig.OscarSeason.AttackSpeedMultiplier
        };
    }

    private static void ApplyStatsOverrides(StatsConfig runtimeConfig, StatsConfigData stats)
    {
        if (stats == null)
            return;

        if (stats.BaseHealth.HasValue)
            runtimeConfig.BaseHealth = stats.BaseHealth.Value;

        if (stats.StartingMoney.HasValue)
            runtimeConfig.StartingMoney = stats.StartingMoney.Value;

        if (stats.SpamPunishment?.PenaltyPerTier.HasValue == true)
            runtimeConfig.SpamPenaltyPerTier = stats.SpamPunishment.PenaltyPerTier.Value;

        ApplyRewardOverrides(runtimeConfig.Rewards, stats.Rewards);
        ApplyPopularityOverrides(runtimeConfig.Popularity, stats.Popularity);
        ApplyOscarSeasonOverrides(runtimeConfig.OscarSeason, stats.OscarSeason);
    }

    private static void ApplyRewardOverrides(RewardConfig runtimeConfig, RewardConfigData overrides)
    {
        if (overrides == null)
            return;

        if (overrides.MeleeDeathPoints.HasValue)
            runtimeConfig.MeleeDeathPoints = overrides.MeleeDeathPoints.Value;

        if (overrides.RangedDeathPoints.HasValue)
            runtimeConfig.RangedDeathPoints = overrides.RangedDeathPoints.Value;

        if (overrides.HeavyDeathPoints.HasValue)
            runtimeConfig.HeavyDeathPoints = overrides.HeavyDeathPoints.Value;

        if (overrides.CostMultiplier.HasValue)
            runtimeConfig.CostMultiplier = overrides.CostMultiplier.Value;
    }

    private static void ApplyPopularityOverrides(PopularityConfig runtimeConfig, PopularityConfigData overrides)
    {
        if (overrides == null)
            return;

        if (overrides.StartingPoints.HasValue)
            runtimeConfig.StartingPoints = overrides.StartingPoints.Value;

        if (overrides.MaxTier.HasValue)
            runtimeConfig.MaxTier = overrides.MaxTier.Value;

        if (overrides.BaseLimit.HasValue)
            runtimeConfig.BaseLimit = overrides.BaseLimit.Value;

        if (overrides.LinearLimitGrowth.HasValue)
            runtimeConfig.LinearLimitGrowth = overrides.LinearLimitGrowth.Value;

        if (overrides.QuadraticLimitGrowth.HasValue)
            runtimeConfig.QuadraticLimitGrowth = overrides.QuadraticLimitGrowth.Value;
    }

    private static void ApplyOscarSeasonOverrides(OscarSeasonConfig runtimeConfig, OscarSeasonConfigData overrides)
    {
        if (overrides == null)
            return;

        if (overrides.Duration.HasValue)
            runtimeConfig.Duration = overrides.Duration.Value;

        if (overrides.MoveSpeedMultiplier.HasValue)
            runtimeConfig.MoveSpeedMultiplier = overrides.MoveSpeedMultiplier.Value;

        if (overrides.AttackSpeedMultiplier.HasValue)
            runtimeConfig.AttackSpeedMultiplier = overrides.AttackSpeedMultiplier.Value;
    }

    private static Dictionary<string, UnitType> BuildRuntimeUnits(
        IReadOnlyDictionary<string, UnitType> defaultUnits, 
        List<UnitConfigData> unitOverrides
    )
    {
        Dictionary<string, UnitConfigData> overrides = BuildUnitOverrideRegistry(unitOverrides);
        Dictionary<string, UnitType> runtimeUnits = new();

        foreach (KeyValuePair<string, UnitType> pair in defaultUnits)
        {
            UnitType runtimeUnit = Object.Instantiate(pair.Value);
            runtimeUnit.name = $"{pair.Value.name} (Runtime)";

            if (overrides.TryGetValue(pair.Key, out UnitConfigData unitOverride))
                ApplyUnitOverrides(runtimeUnit, unitOverride);

            runtimeUnits.Add(pair.Key, runtimeUnit);
        }

        return runtimeUnits;
    }

    private static Dictionary<string, UnitConfigData> BuildUnitOverrideRegistry(List<UnitConfigData> unitOverrides)
    {
        Dictionary<string, UnitConfigData> overrides = new();

        if (unitOverrides == null)
            return overrides;

        foreach (UnitConfigData unitOverride in unitOverrides)
        {
            if (unitOverride == null || string.IsNullOrWhiteSpace(unitOverride.Id))
                continue;

            overrides[unitOverride.Id] = unitOverride;
        }

        return overrides;
    }

    private static void ApplyUnitOverrides(UnitType runtimeUnit, UnitConfigData overrides)
    {
        if (overrides.Cost.HasValue)
            runtimeUnit.Cost = overrides.Cost.Value;

        if (overrides.Damage.HasValue)
            runtimeUnit.Damage = overrides.Damage.Value;

        if (overrides.MaxHealth.HasValue)
            runtimeUnit.MaxHealth = overrides.MaxHealth.Value;

        if (overrides.AttackRange.HasValue)
            runtimeUnit.AttackRange = overrides.AttackRange.Value;

        if (overrides.AttackCooldown.HasValue)
            runtimeUnit.AttackCooldown = overrides.AttackCooldown.Value;

        if (overrides.MoveSpeed.HasValue)
            runtimeUnit.MoveSpeed = overrides.MoveSpeed.Value;
    }

    private static Dictionary<string, GenreNode> BuildRuntimeGenres(
    IReadOnlyDictionary<string, GenreNode> defaultGenres,
    Dictionary<string, UnitType> runtimeUnits)
    {
        Dictionary<string, GenreNode> runtimeGenres = new();

        foreach (KeyValuePair<string, GenreNode> pair in defaultGenres)
        {
            GenreNode runtimeGenre = Object.Instantiate(pair.Value);
            runtimeGenre.name = $"{pair.Value.name} (Runtime)";

            runtimeGenres.Add(pair.Key, runtimeGenre);
        }

        foreach (KeyValuePair<string, GenreNode> pair in defaultGenres)
            RelinkGenre(pair.Value, runtimeGenres[pair.Key], runtimeUnits, runtimeGenres);

        return runtimeGenres;
    }

    private static void RelinkGenre(
        GenreNode defaultGenre,
        GenreNode runtimeGenre,
        Dictionary<string, UnitType> runtimeUnits,
        Dictionary<string, GenreNode> runtimeGenres
    )
    {
        runtimeGenre.MeleeUnit = GetRuntimeUnit(defaultGenre.MeleeUnit, runtimeUnits);
        runtimeGenre.RangedUnit = GetRuntimeUnit(defaultGenre.RangedUnit, runtimeUnits);
        runtimeGenre.HeavyUnit = GetRuntimeUnit(defaultGenre.HeavyUnit, runtimeUnits);

        runtimeGenre.Children = new List<GenreNode>();

        foreach (GenreNode child in defaultGenre.Children)
            runtimeGenre.Children.Add(runtimeGenres[child.Id]);
    }

    private static UnitType GetRuntimeUnit(UnitType defaultUnit, Dictionary<string, UnitType> runtimeUnits)
    {
        if (defaultUnit == null)
            return null;

        return runtimeUnits[defaultUnit.Id];
    }
}