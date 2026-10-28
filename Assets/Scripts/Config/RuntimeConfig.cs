using System.Collections.Generic;

public class RuntimeConfig
{
    public StatsConfig Stats { get; }
    public IReadOnlyDictionary<string, UnitType> Units { get; }
    public IReadOnlyDictionary<string, GenreNode> Genres { get; }

    public RuntimeConfig(StatsConfig stats, Dictionary<string, UnitType> units, Dictionary<string, GenreNode> genres)
    {
        Stats = stats;
        Units = units;
        Genres = genres;
    }
}