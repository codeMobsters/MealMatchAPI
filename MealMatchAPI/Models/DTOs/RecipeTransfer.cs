namespace MealMatchAPI.Models.DTOs;

public class RecipeTransfer
{
    public int RecipeId { get; set; }
    public string Title { get; set; }
    public int? Like { get; set; }
    public int? Yield { get; set; }
    public double? Calories { get; set; }
    public double TotalTime { get; set; }
    public DateTime? CreatedAt { get; set; }
    
    public List<string> Ingredients { get; set; }
    public List<string>? CuisineType { get; set; }
    public List<string>? DietLabels { get; set; }
    public List<string>? DishType { get; set; }
    public List<string>? HealthLabels { get; set; }
    public List<string>? MealType { get; set; }

    public string PictureUrl { get; set; }
    public string? SourceUrl { get; set; }

    public int UserId { get; set; } // Author of the recipe
    public string? Username { get; set; } // Author of the recipe
    public User User { get; set; }
    public List<Comment>? Comments { get; set; }
}