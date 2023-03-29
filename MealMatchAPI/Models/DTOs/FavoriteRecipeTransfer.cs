using System.Text.Json.Serialization;

namespace MealMatchAPI.Models.DTOs;

public class FavoriteRecipeTransfer
{
    public int FavoriteRecipeId { get; set; }

    public RecipeTransfer Recipe { get; set; }
    
    [JsonPropertyName("favoriteRecipeOwnerId")]
    public int RecipeUserId { get; set; } // Owner of the favorite recipe
    [JsonPropertyName("favoriteRecipeOwnerName")]
    public string? UserName { get; set; } // Name of the owner of the favorite recipe
}