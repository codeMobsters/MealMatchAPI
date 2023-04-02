namespace MealMatchAPI.Models.DTOs;

public class LoginResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Token { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public List<string>? ProfileSettings { get; set; }
    public List<string>? DietLabels { get; set; }
    public List<string>? HealthLabels { get; set; }
    
    public List<int>? FavoriteRecipes { get; set; }
    public List<int>? LikedRecipes { get; set; }
    public List<int>? FollowedUserIds { get; set; }
}