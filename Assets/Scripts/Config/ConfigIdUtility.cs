using System.Text.RegularExpressions;

public static class ConfigIdUtility
{
    public static string ToKebabCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        string kebab = Regex.Replace(value.Trim(), @"([a-z0-9])([A-Z])", "$1-$2");
        kebab = Regex.Replace(kebab.ToLowerInvariant(), @"[^a-z0-9]+", "-");

        return kebab.Trim('-');
    }
}