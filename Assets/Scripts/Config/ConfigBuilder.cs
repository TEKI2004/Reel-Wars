using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class ConfigBuilder
{
    public static async Task<RuntimeConfig> BuildAsync(StatsConfig defaultConfig, ConfigData config, UnitBehaviour unitTemplate)
    {
        StatsConfig stats = BuildStats(defaultConfig, config?.Stats);

        DefaultAssetRegistry registry = DefaultAssetRegistry.Build(defaultConfig.StartingGenre, defaultConfig.Popularity.MaxTier);

        Dictionary<string, UnitType> units = await BuildUnitsAsync(registry.Units, config?.Units, unitTemplate);
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

    private static async Task<Dictionary<string, UnitType>> BuildUnitsAsync(
        IReadOnlyDictionary<string, UnitType> defaults,
        List<UnitConfigData> definitions,
        UnitBehaviour unitTemplate
    )
    {
        Dictionary<string, UnitType> units = new();

        foreach (KeyValuePair<string, UnitType> pair in defaults)
        {
            UnitType unit = Object.Instantiate(pair.Value);
            unit.name = $"{pair.Value.UnitName} (Runtime)";

            units.Add(pair.Key, unit);
        }

        if (definitions == null)
            return units;

        foreach (UnitConfigData definition in definitions)
        {
            if (units.TryGetValue(definition.Id, out UnitType existingUnit))
            {
                ApplyUnit(existingUnit, definition);
                existingUnit.name = $"{existingUnit.UnitName} (Runtime)";
                continue;
            }

            UnitType newUnit = await CreateNewUnitAsync(definition, unitTemplate);
            units.Add(definition.Id, newUnit);
        }

        return units;
    }

    private static async Task<UnitType> CreateNewUnitAsync(UnitConfigData definition, UnitBehaviour unitTemplate)
    {
        UnitType unit = ScriptableObject.CreateInstance<UnitType>();

        unit.SetRuntimeId(definition.Id);
        ApplyUnit(unit, definition);

        unit.Icon = ExternalAssetLoader.LoadSprite(ConfigLoader.GetUnitIconPath(definition.Id));
        unit.UnitPrefab = await ExternalAssetLoader.CreateUnitPrefabAsync(
            unitTemplate,
            ConfigLoader.GetModelPath(definition.Id),
            definition.Id
        );

        unit.name = $"{unit.UnitName} (Runtime)";

        return unit;
    }

    private static void ApplyUnit(UnitType target, UnitConfigData overrides)
    {
        if (overrides.Name != null)
            target.UnitName = overrides.Name;

        if (overrides.Role.HasValue)
            target.Role = overrides.Role.Value;

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
        List<GenreConfigData> definitions
    )
    {
        Dictionary<string, GenreConfigData> definitionsById = new();

        if (definitions != null)
        {
            foreach (GenreConfigData definition in definitions)
                definitionsById.Add(definition.Id, definition);
        }

        Dictionary<string, GenreNode> genres = new();

        foreach (KeyValuePair<string, GenreNode> pair in defaults)
        {
            GenreNode genre = Object.Instantiate(pair.Value);
            genre.name = $"{pair.Value.GenreName} (Runtime)";

            genres.Add(pair.Key, genre);
        }

        if (definitions != null)
        {
            foreach (GenreConfigData definition in definitions)
            {
                if (!genres.ContainsKey(definition.Id))
                    genres.Add(definition.Id, CreateNewGenre(definition));
            }
        }

        foreach (KeyValuePair<string, GenreNode> pair in defaults)
        {
            definitionsById.TryGetValue(pair.Key, out GenreConfigData overrides);
            ApplyExistingGenre(pair.Value, genres[pair.Key], overrides, units, genres);
        }

        if (definitions != null)
        {
            foreach (GenreConfigData definition in definitions)
            {
                if (!defaults.ContainsKey(definition.Id))
                    ApplyNewGenre(genres[definition.Id], definition, units, genres);
            }
        }

        return genres;
    }

    private static GenreNode CreateNewGenre(GenreConfigData definition)
    {
        GenreNode genre = ScriptableObject.CreateInstance<GenreNode>();

        genre.SetRuntimeId(definition.Id);
        genre.GenreName = definition.Name;
        genre.PassiveDescription = definition.PassiveDescription;
        genre.Icon = ExternalAssetLoader.LoadSprite(ConfigLoader.GetGenreIconPath(definition.Id));
        genre.Children = new List<GenreNode>();
        genre.name = $"{genre.GenreName} (Runtime)";

        return genre;
    }

    private static void ApplyExistingGenre(
        GenreNode defaults,
        GenreNode target,
        GenreConfigData overrides,
        Dictionary<string, UnitType> units,
        Dictionary<string, GenreNode> genres
    )
    {
        if (overrides?.Name != null)
            target.GenreName = overrides.Name;

        if (overrides?.PassiveDescription != null)
            target.PassiveDescription = overrides.PassiveDescription;

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
        }
        else
        {
            foreach (GenreNode child in defaults.Children)
                target.Children.Add(genres[child.Id]);
        }

        target.name = $"{target.GenreName} (Runtime)";
    }

    private static void ApplyNewGenre(
        GenreNode target,
        GenreConfigData definition,
        Dictionary<string, UnitType> units,
        Dictionary<string, GenreNode> genres
    )
    {
        target.MeleeUnit = units[definition.MeleeUnitId];
        target.RangedUnit = units[definition.RangedUnitId];
        target.HeavyUnit = units[definition.HeavyUnitId];

        target.Children = new List<GenreNode>();

        foreach (string childId in definition.Children)
            target.Children.Add(genres[childId]);
    }
}