using System.Net.Http.Json;
using System.Globalization;
using System.Text.RegularExpressions;
using Recept.Models;

namespace Recept.Services;

public class RecipeService(HttpClient http)
{
    private readonly HttpClient _http = http;
    private List<RecipeMetadata>? _cachedRecipes;
    private readonly Dictionary<string, string> _markdownCache = new(StringComparer.OrdinalIgnoreCase);

    public async Task<List<RecipeMetadata>> GetAllRecipesAsync()
    {
        if (_cachedRecipes is not null)
            return _cachedRecipes;

        try
        {
            var slugs = await _http.GetFromJsonAsync<List<string>>("recipes/recipes.json");
            if (slugs == null) return [];

            var recipes = new List<RecipeMetadata>();

            foreach (var slug in slugs)
            {
                try
                {
                    var markdown = await GetRecipeMarkdownAsync(slug);
                    recipes.Add(ParseMetadata(slug, markdown));
                }
                catch
                {
                    // Skip recipes that fail to load
                    continue;
                }
            }

            _cachedRecipes = recipes;
            return recipes;
        }
        catch
        {
            return [];
        }
    }

    public async Task<RecipeMetadata> GetRecipeMetadataAsync(string slug)
    {
        var markdown = await GetRecipeMarkdownAsync(slug);
        return ParseMetadata(slug, markdown);
    }

    public async Task<string> GetRecipeMarkdownAsync(string slug)
    {
        if (_markdownCache.TryGetValue(slug, out var cached))
            return cached;

        var markdown = await _http.GetStringAsync($"recipes/{slug}.md");
        _markdownCache[slug] = markdown;
        return markdown;
    }

    public static RecipeMetadata ParseMetadata(string slug, string markdown)
    {
        var metadata = new RecipeMetadata { Slug = slug };

        var frontmatter = ExtractFrontmatter(markdown);
        if (frontmatter is null) return metadata;

        // Parse title
        var titleMatch = Regex.Match(frontmatter, @"title:\s*[""'](.+?)[""']");
        if (titleMatch.Success)
        {
            metadata.Title = titleMatch.Groups[1].Value;
        }

        // Parse created date
        var createdMatch = Regex.Match(frontmatter, @"created:\s*[\""']?(\d{4}-\d{2}-\d{2})[\""']?");
        if (createdMatch.Success &&
            DateOnly.TryParseExact(createdMatch.Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var created))
        {
            metadata.Created = created;
        }

        // Parse servings
        var servingsMatch = Regex.Match(frontmatter, @"servings:\s*(\d+)");
        if (servingsMatch.Success && int.TryParse(servingsMatch.Groups[1].Value, out int servings))
        {
            metadata.Servings = servings;
        }

        // Parse cook time
        var cookTimeMatch = Regex.Match(frontmatter, @"cookTime:\s*[""']?(.+?)[""']?\s*$", RegexOptions.Multiline);
        if (cookTimeMatch.Success)
        {
            metadata.CookTime = cookTimeMatch.Groups[1].Value.Trim();
        }

        // Parse categories
        var categories = ParseFrontmatterList(frontmatter, "categories")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Backwards compatibility for old frontmatter
        var freezableMatch = Regex.Match(frontmatter, @"freezable:\s*(true|false)", RegexOptions.IgnoreCase);
        var isFreezable = freezableMatch.Success &&
                          string.Equals(freezableMatch.Groups[1].Value, "true", StringComparison.OrdinalIgnoreCase);
        if (isFreezable && !categories.Contains("Frysbara"))
        {
            categories.Add("Frysbara");
        }

        metadata.Categories = categories;

        // Parse ingredients tags
        metadata.Ingredients = ParseFrontmatterList(frontmatter, "ingredients");

        return metadata;
    }

    private static readonly Regex FrontmatterRegex = new(
        @"^---\s*\r?\n(.*?)\r?\n---",
        RegexOptions.Singleline | RegexOptions.Compiled);

    public static string? ExtractFrontmatter(string markdown)
    {
        var match = FrontmatterRegex.Match(markdown);
        return match.Success ? match.Groups[1].Value : null;
    }

    public static string StripFrontmatter(string markdown)
    {
        var match = FrontmatterRegex.Match(markdown);
        if (!match.Success) return markdown;
        return markdown[match.Length..].TrimStart('\r', '\n');
    }

    public static List<string> ParseFrontmatterList(string frontmatter, string key)
    {
        var listMatch = Regex.Match(
            frontmatter,
            $@"{Regex.Escape(key)}:\s*\r?\n((?:\s+-\s+.+\r?\n?)+)",
            RegexOptions.Multiline);

        if (!listMatch.Success) return [];

        return Regex.Matches(listMatch.Groups[1].Value, @"-\s+(.+)")
            .Select(m => m.Groups[1].Value.Trim())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToList();
    }
}
