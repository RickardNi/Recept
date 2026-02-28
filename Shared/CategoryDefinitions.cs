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

    public static string GetBadgeCssClass(string category)
    {
        if (string.Equals(category, "Veganskt", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(category, "Vegetariskt", StringComparison.OrdinalIgnoreCase))
        {
            return "recipe-badge recipe-badge--green";
        }

        if (string.Equals(category, "Frysbara", StringComparison.OrdinalIgnoreCase))
        {
            return "recipe-badge recipe-badge--freezable";
        }

        return "recipe-badge";
    }

    public static string GetCategoryLink(string category)
    {
        return $"categories?category={Uri.EscapeDataString(category)}";
    }

    public static string GetIngredientLink(string ingredient)
    {
        return $"ingredients?ingredient={Uri.EscapeDataString(ingredient)}";
    }

    public static OrderedCategories GetOrderedCategories(List<string> categories)
    {
        var highlighted = HighlightedCategoryOrder
            .Where(highlight => categories.Contains(highlight, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (highlighted.Any(category => string.Equals(category, "Veganskt", StringComparison.OrdinalIgnoreCase)))
        {
            highlighted = highlighted
                .Where(category => !string.Equals(category, "Vegetariskt", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var secondary = categories
            .Where(category => !HighlightedCategorySet.Contains(category))
            .ToList();

        return new OrderedCategories(highlighted, secondary);
    }
}

public sealed record OrderedCategories(List<string> Highlighted, List<string> Secondary);