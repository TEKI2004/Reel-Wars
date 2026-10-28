using System.Collections.Generic;
using UnityEngine;

public static class ConfigBuilder
{
    public static RuntimeConfig Build(StatsConfig defaultConfig, ConfigData config)
    {
        StatsConfig stats = BuildStats(defaultConfig, config?.Stats);

        DefaultAssetRegistry registry = DefaultAssetRegistry.Build(
            defaultConfig.StartingGenre,
            stats.Popularity.MaxTier
        );

        Dictionary<string, UnitType> units = BuildUnits(registry.Units, config?.Units);
        Dictionary<string, GenreNode> genres = BuildGenres(registry.Genres, units, config?.Genres);

        string startingGenreId = config?.Stats?.StartingGenreId ?? defaultConfig.StartingGenre.Id;
        stats.StartingGenre = genres[startingGenreId];

        return new RuntimeConfig(stats, units, genres);
    }

    // ─────────────────────────────────────────────
    // Stats
    // ─────────────────────────────────────────────

    private static StatsConfig BuildStats(StatsConfig defaults, StatsConfigData overrides)
    {
        StatsConfig stats = Object.Instantiate(defaults);
        stats.name = $"{defaults.name} (Runtime)";

        stats.Rewards = new RewardConfig
        {
            MeleeDeathPoints = defaults.Rewards.MeleeDeathPoints,
            RangedDeathPoints = defaults.Rewards.RangedDeathPoints,
            HeavyDeathPoints = defaults.Rewards.HeavyDeathPoints,
            CostMultiplier = defaults.Rewards.CostMultiplier
        };

        stats.Popularity = new PopularityConfig
        {
            StartingPoints = defaults.Popularity.StartingPoints,
            MaxTier = defaults.Popularity.MaxTier,
            BaseLimit = defaults.Popularity.BaseLimit,
            LinearLimitGrowth = defaults.Popularity.LinearLimitGrowth,
            QuadraticLimitGrowth = defaults.Popularity.QuadraticLimitGrowth
        };

        stats.OscarSeason = new OscarSeasonConfig
        {
            Duration = defaults.OscarSeason.Duration,
            MoveSpeedMultiplier = defaults.OscarSeason.MoveSpeedMultiplier,
            AttackSpeedMultiplier = defaults.OscarSeason.AttackSpeedMultiplier
        };

        if (overrides == null)
            return stats;

        if (overrides.BaseHealth.HasValue)
            stats.BaseHealth = overrides.BaseHealth.Value;

        if (overrides.StartingMoney.HasValue)
            stats.StartingMoney = overrides.StartingMoney.Value;

        if (overrides.SpamPunishment?.PenaltyPerTier.HasValue == true)
            stats.SpamPenaltyPerTier = overrides.SpamPunishment.PenaltyPerTier.Value;

        ApplyRewards(stats.Rewards, overrides.Rewards);
        ApplyPopularity(stats.Popularity, overrides.Popularity);
        ApplyOscarSeason(stats.OscarSeason, overrides.OscarSeason);

        return stats;
    }

    private static void ApplyRewards(RewardConfig target, RewardConfigData overrides)
    {
        if (overrides == null)
            return;

        if (overrides.MeleeDeathPoints.HasValue)
            target.MeleeDeathPoints = overrides.MeleeDeathPoints.Value;

        if (overrides.RangedDeathPoints.HasValue)
            target.RangedDeathPoints = overrides.RangedDeathPoints.Value;

        if (overrides.HeavyDeathPoints.HasValue)
            target.HeavyDeathPoints = overrides.HeavyDeathPoints.Value;

        if (overrides.CostMultiplier.HasValue)
            target.CostMultiplier = overrides.CostMultiplier.Value;
    }

    private static void ApplyPopularity(PopularityConfig target, PopularityConfigData overrides)
    {
        if (overrides == null)
            return;

        if (overrides.StartingPoints.HasValue)
            target.StartingPoints = overrides.StartingPoints.Value;

        if (overrides.MaxTier.HasValue)
            target.MaxTier = overrides.MaxTier.Value;

        if (overrides.BaseLimit.HasValue)
            target.BaseLimit = overrides.BaseLimit.Value;

        if (overrides.LinearLimitGrowth.HasValue)
            target.LinearLimitGrowth = overrides.LinearLimitGrowth.Value;

        if (overrides.QuadraticLimitGrowth.HasValue)
            target.QuadraticLimitGrowth = overrides.QuadraticLimitGrowth.Value;
    }

    private static void ApplyOscarSeason(OscarSeasonConfig target, OscarSeasonConfigData overrides)
    {
        if (overrides == null)
            return;

        if (overrides.Duration.HasValue)
            target.Duration = overrides.Duration.Value;

        if (overrides.MoveSpeedMultiplier.HasValue)
            target.MoveSpeedMultiplier = overrides.MoveSpeedMultiplier.Value;

        if (overrides.AttackSpeedMultiplier.HasValue)
            target.AttackSpeedMultiplier = overrides.AttackSpeedMultiplier.Value;
    }

    // ─────────────────────────────────────────────
    // Units
    // ─────────────────────────────────────────────

    private static Dictionary<string, UnitType> BuildUnits(
        IReadOnlyDictionary<string, UnitType> defaults,
        List<UnitConfigData> overrides
    )
    {
        Dictionary<string, UnitConfigData> overrideLookup = new();

        if (overrides != null)
        {
            foreach (UnitConfigData unit in overrides)
                overrideLookup.Add(unit.Id, unit);
        }

        Dictionary<string, UnitType> units = new();

        foreach (KeyValuePair<string, UnitType> pair in defaults)
        {
            UnitType unit = Object.Instantiate(pair.Value);
            unit.name = $"{pair.Value.name} (Runtime)";

            if (overrideLookup.TryGetValue(pair.Key, out UnitConfigData unitOverride))
                ApplyUnit(unit, unitOverride);

            units.Add(pair.Key, unit);
        }

        return units;
    }

    private static void ApplyUnit(UnitType target, UnitConfigData overrides)
    {
        if (overrides.Cost.HasValue)
            target.Cost = overrides.Cost.Value;

        if (overrides.Damage.HasValue)
            target.Damage = overrides.Damage.Value;

        if (overrides.MaxHealth.HasValue)
            target.MaxHealth = overrides.MaxHealth.Value;

        if (overrides.AttackRange.HasValue)
            target.AttackRange = overrides.AttackRange.Value;

        if (overrides.AttackCooldown.HasValue)
            target.AttackCooldown = overrides.AttackCooldown.Value;

        if (overrides.MoveSpeed.HasValue)
            target.MoveSpeed = overrides.MoveSpeed.Value;
    }

    // ─────────────────────────────────────────────
    // Genres
    // ─────────────────────────────────────────────

    private static Dictionary<string, GenreNode> BuildGenres(
        IReadOnlyDictionary<string, GenreNode> defaults,
        Dictionary<string, UnitType> units,
        List<GenreConfigData> overrides
    )
    {
        Dictionary<string, GenreConfigData> overrideLookup = new();

        if (overrides != null)
        {
            foreach (GenreConfigData genre in overrides)
                overrideLookup.Add(genre.Id, genre);
        }

        Dictionary<string, GenreNode> genres = new();

        foreach (KeyValuePair<string, GenreNode> pair in defaults)
        {
            GenreNode genre = Object.Instantiate(pair.Value);
            genre.name = $"{pair.Value.name} (Runtime)";

            genres.Add(pair.Key, genre);
        }

        foreach (KeyValuePair<string, GenreNode> pair in defaults)
        {
            overrideLookup.TryGetValue(pair.Key, out GenreConfigData genreOverride);
            ApplyGenre(pair.Value, genres[pair.Key], genreOverride, units, genres);
        }

        return genres;
    }

    private static void ApplyGenre(
        GenreNode defaults,
        GenreNode target,
        GenreConfigData overrides,
        Dictionary<string, UnitType> units,
        Dictionary<string, GenreNode> genres
    )
    {
        string meleeId = overrides?.MeleeUnitId ?? defaults.MeleeUnit.Id;
        string rangedId = overrides?.RangedUnitId ?? defaults.RangedUnit.Id;
        string heavyId = overrides?.HeavyUnitId ?? defaults.HeavyUnit.Id;

        target.MeleeUnit = units[meleeId];
        target.RangedUnit = units[rangedId];
        target.HeavyUnit = units[heavyId];

        target.Children = new List<GenreNode>();

        if (overrides?.Children != null)
        {
            foreach (string childId in overrides.Children)
                target.Children.Add(genres[childId]);

            return;
        }

        foreach (GenreNode child in defaults.Children)
            target.Children.Add(genres[child.Id]);
    }
}