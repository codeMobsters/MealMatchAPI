using System.Text.Json.Serialization;

namespace MealMatchAPI.Models.DTOs;

public class RecipeTransfer
{
    public int? RecipeId { get; set; }
    [JsonPropertyName("label")]
    public string Title { get; set; }
    public int? Like { get; set; }
    [JsonPropertyName("yield")]
    public double? Yield { get; set; }
    [JsonPropertyName("calories")]
    public double? Calories { get; set; }
    [JsonPropertyName("totalTime")]
    public double TotalTime { get; set; }
    public DateTime? CreatedAt { get; set; }
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
    
    [JsonPropertyName("image")]
    public string PictureUrl { get; set; }
    [JsonPropertyName("url")]
    public string? SourceUrl { get; set; }

    public int UserId { get; set; } // Author of the recipe
    public string? Username { get; set; } // Author of the recipe
    public List<Comment>? Comments { get; set; }
}