namespace MealMatchAPI.Models.DTOs;

public class EdamamResponse
{
    public string? Url { get; set; }
    public List<RecipeTransfer>? Recipes { get; set; }
}