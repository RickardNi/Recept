using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Recept.Models;
using Recept.Shared;

namespace Recept.Services;

public class RecipeService(HttpClient http)
{
    private readonly HttpClient _http = http;

    public async Task<List<RecipeMetadata>> GetAllRecipesAsync()
    {
        try
        {
            // Fetch the list of recipe slugs
            var slugs = await _http.GetFromJsonAsync<List<string>>("recipes/recipes.json");
            if (slugs == null) return [];

            var recipes = new List<RecipeMetadata>();

            // Fetch metadata for each recipe
            foreach (var slug in slugs)
            {
                try
                {
                    var metadata = await GetRecipeMetadataAsync(slug);
                    recipes.Add(metadata);
                }
                catch
                {
                    // Skip recipes that fail to load
                    continue;
                }
            }

            return recipes;
        }
        catch
        {
            return [];
        }
    }

    public async Task<RecipeMetadata> GetRecipeMetadataAsync(string slug)
    {
        var markdown = await _http.GetStringAsync($"recipes/{slug}.md");
        return ParseMetadata(slug, markdown);
    }

    private static RecipeMetadata ParseMetadata(string slug, string markdown)
    {
        var metadata = new RecipeMetadata { Slug = slug };

        // Check for YAML frontmatter
        var frontmatterMatch = Regex.Match(markdown, @"^---\s*\n(.*?)\n---", RegexOptions.Singleline);

        if (frontmatterMatch.Success)
        {
            var frontmatter = frontmatterMatch.Groups[1].Value;

            // Parse title
            var titleMatch = Regex.Match(frontmatter, @"title:\s*[""'](.+?)[""']");
            if (titleMatch.Success)
            {
                metadata.Title = titleMatch.Groups[1].Value;
            }

            // Parse servings
            var servingsMatch = Regex.Match(frontmatter, @"servings:\s*(\d+)");
            if (servingsMatch.Success && int.TryParse(servingsMatch.Groups[1].Value, out int servings))
            {
                metadata.Servings = servings;
            }

            // Parse categories
            var categoriesMatch = Regex.Match(frontmatter, @"categories:\s*\n((?:\s+-\s+.+\n?)+)", RegexOptions.Multiline);
            if (categoriesMatch.Success)
            {
                metadata.Categories = Regex.Matches(categoriesMatch.Groups[1].Value, @"-\s+(.+)")
                    .Select(m => m.Groups[1].Value.Trim())
                    .ToList();

                metadata.Categories = CategoryDefinitions.NormalizeMany(metadata.Categories);
            }

            // Backwards compatibility for old frontmatter
            var freezableMatch = Regex.Match(frontmatter, @"freezable:\s*(true|false)", RegexOptions.IgnoreCase);
            var isFreezable = freezableMatch.Success &&
                              string.Equals(freezableMatch.Groups[1].Value, "true", StringComparison.OrdinalIgnoreCase);
            if (isFreezable && !metadata.Categories.Contains("Frysbara"))
            {
                metadata.Categories.Add("Frysbara");
            }

            // Parse ingredients tags
            var ingredientsMatch = Regex.Match(frontmatter, @"ingredients:\s*\n((?:\s+-\s+.+\n?)+)", RegexOptions.Multiline);
            if (ingredientsMatch.Success)
            {
                metadata.Ingredients = Regex.Matches(ingredientsMatch.Groups[1].Value, @"-\s+(.+)")
                    .Select(m => m.Groups[1].Value.Trim())
                    .ToList();
            }
        }

        return metadata;
    }
}
