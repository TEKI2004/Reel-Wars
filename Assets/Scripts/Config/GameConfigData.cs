using System.Collections.Generic;

public class GameConfigData
{
    public int? SchemaVersion { get; set; }

    public StatsConfigData Stats { get; set; }
    public List<UnitConfigData> Units { get; set; }
}

public class StatsConfigData
{
    public int? BaseHealth { get; set; }
    public int? StartingMoney { get; set; }
    public string StartingGenreId { get; set; }

    public RewardConfigData Rewards { get; set; }
    public SpamPunishmentConfigData SpamPunishment { get; set; }
    public PopularityConfigData Popularity { get; set; }
    public OscarSeasonConfigData OscarSeason { get; set; }
}

public class RewardConfigData
{
    public int? MeleeDeathPoints { get; set; }
    public int? RangedDeathPoints { get; set; }
    public int? HeavyDeathPoints { get; set; }
    public float? CostMultiplier { get; set; }
}

public class SpamPunishmentConfigData
{
    public int? PenaltyPerTier { get; set; }
}

public class PopularityConfigData
{
    public int? StartingPoints { get; set; }
    public int? MaxTier { get; set; }

    public int? BaseLimit { get; set; }
    public int? LinearLimitGrowth { get; set; }
    public int? QuadraticLimitGrowth { get; set; }
}

public class OscarSeasonConfigData
{
    public float? Duration { get; set; }
    public float? MoveSpeedMultiplier { get; set; }
    public float? AttackSpeedMultiplier { get; set; }
}

public class UnitConfigData
{
    public string Id { get; set; }

    public int? Cost { get; set; }
    public int? Damage { get; set; }
    public int? MaxHealth { get; set; }

    public float? AttackRange { get; set; }
    public float? AttackCooldown { get; set; }
    public float? MoveSpeed { get; set; }
}