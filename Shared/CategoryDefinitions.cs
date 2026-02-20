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