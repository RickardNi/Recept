namespace Recept.Models;

public class RecipeMetadata
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int? Servings { get; set; }
    public List<string> Categories { get; set; } = new();
    public List<string> Ingredients { get; set; } = new();
}
