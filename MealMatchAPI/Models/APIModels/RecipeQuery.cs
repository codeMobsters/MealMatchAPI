namespace MealMatchAPI.Models.APIModels;

public class RecipeQuery
{
    public List<string>? CuisineType { get; set; }
    public List<string>? DietLabels { get; set; }
    public List<string>? DishType { get; set; }
    public List<string>? HealthLabels { get; set; }
    public List<string>? MealType { get; set; }
}