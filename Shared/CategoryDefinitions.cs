namespace Recept.Shared;

public static class CategoryDefinitions
{
    public static readonly string[] HighlightedCategoryOrder = ["Veganskt", "Vegetariskt", "Frysbara"];

    public static readonly HashSet<string> HighlightedCategorySet =
    [
        "Veganskt",
        "Vegetariskt",
        "Frysbara"
    ];

    public static string Normalize(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return string.Empty;
        }

        var value = category.Trim();

        if (string.Equals(value, "Frysbar", StringComparison.OrdinalIgnoreCase))
        {
            return "Frysbara";
        }

        return value;
    }

    public static List<string> NormalizeMany(IEnumerable<string> categories)
    {
        return categories
            .Select(Normalize)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static string GetBadgeLabel(string category)
    {
        if (string.Equals(category, "Veganskt", StringComparison.OrdinalIgnoreCase))
        {
            return "Vegansk";
        }

        if (string.Equals(category, "Vegetariskt", StringComparison.OrdinalIgnoreCase))
        {
            return "Vegetarisk";
        }

        if (string.Equals(category, "Frysbara", StringComparison.OrdinalIgnoreCase))
        {
            return "Frysbar";
        }

        return category;
    }
}