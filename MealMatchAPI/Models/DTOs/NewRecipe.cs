using System.Text.Json.Serialization;

namespace MealMatchAPI.Models.DTOs;

public class NewRecipe
{
    [JsonPropertyName("label")]
    public string Title { get; set; }
    [JsonPropertyName("yield")]
    public double? Yield { get; set; }
    [JsonPropertyName("calories")]
    public double? Calories { get; set; }
    [JsonPropertyName("totalTime")]
    public double? TotalTime { get; set; }
    [JsonPropertyName("instructions")]
    public List<string> Instructions { get; set; }
    [JsonPropertyName("ingredientLines")]
    public List<string> Ingredients { get; set; }
    [JsonPropertyName("cuisineType")]
    public List<string>? CuisineType { get; set; }
    [JsonPropertyName("dietLabels")]
    public List<string>? DietLabels { get; set; }
    [JsonPropertyName("dishType")]
    public List<string>? DishType { get; set; }
    [JsonPropertyName("healthLabels")]
    public List<string>? HealthLabels { get; set; }
    [JsonPropertyName("mealType")]
    public List<string>? MealType { get; set; }
    
    public IFormFile RecipePicture { get; set; }
}