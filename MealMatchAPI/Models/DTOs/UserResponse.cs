using System.ComponentModel.DataAnnotations;

namespace MealMatchAPI.Models.DTOs;

public class UserResponse
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public List<string>? ProfileSettings { get; set; }
    public List<string>? DietLabels { get; set; }
    public List<string>? HealthLabels { get; set; }
    public int? FavoriteRecipes { get; set; }
    public int? OwnedRecipes { get; set; }
    
    public int Followers { get; set; }
    public int Following { get; set; }
}