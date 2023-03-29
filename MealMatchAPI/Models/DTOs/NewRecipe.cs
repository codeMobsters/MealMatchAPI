using System.Text.Json.Serialization;

namespace MealMatchAPI.Models.DTOs;

public class NewRecipe
{
    public string Title { get; set; }
    public double? Yield { get; set; }
    public double? Calories { get; set; }
    public double? TotalTime { get; set; }
    public List<string> Instructions { get; set; }
    public List<string> Ingredients { get; set; }
    public List<string>? CuisineType { get; set; }
    public List<string>? DietLabels { get; set; }
    public List<string>? DishType { get; set; }
    public List<string>? HealthLabels { get; set; }
    public List<string>? MealType { get; set; }
    public IFormFile RecipePicture { get; set; }
}