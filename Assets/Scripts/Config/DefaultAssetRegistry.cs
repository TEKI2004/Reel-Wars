using System.Collections.Generic;

public class DefaultAssetRegistry
{
    private readonly Dictionary<string, GenreNode> genres = new();
    private readonly Dictionary<string, UnitType> units = new();
    private readonly List<string> errors = new();

    public IReadOnlyDictionary<string, GenreNode> Genres => genres;
    public IReadOnlyDictionary<string, UnitType> Units => units;
    public IReadOnlyList<string> Errors => errors;

    public static DefaultAssetRegistry Build(GenreNode startingGenre, int maxTier)
    {
        DefaultAssetRegistry registry = new();
        registry.CollectGenre(startingGenre, 1, maxTier);
        return registry;
    }

    private void CollectGenre(GenreNode genre, int depth, int maxTier)
    {
        if (genre == null || depth > maxTier)
            return;

        RegisterGenre(genre);

        RegisterUnit(genre.MeleeUnit);
        RegisterUnit(genre.RangedUnit);
        RegisterUnit(genre.HeavyUnit);

        foreach (GenreNode child in genre.Children)
            CollectGenre(child, depth + 1, maxTier);
    }

    private void RegisterGenre(GenreNode genre)
    {
        if (string.IsNullOrWhiteSpace(genre.Id))
        {
            errors.Add($"Genre '{genre.GenreName}' has no config ID.");
            return;
        }

        if (genres.TryGetValue(genre.Id, out GenreNode existingGenre))
        {
            if (existingGenre != genre)
                errors.Add($"Duplicate genre ID '{genre.Id}'.");

            return;
        }

        genres.Add(genre.Id, genre);
    }

    private void RegisterUnit(UnitType unit)
    {
        if (unit == null)
            return;

        if (string.IsNullOrWhiteSpace(unit.Id))
        {
            errors.Add($"Unit '{unit.UnitName}' has no config ID.");
            return;
        }

        if (units.TryGetValue(unit.Id, out UnitType existingUnit))
        {
            if (existingUnit != unit)
                errors.Add($"Duplicate unit ID '{unit.Id}'.");

            return;
        }

        units.Add(unit.Id, unit);
    }
}