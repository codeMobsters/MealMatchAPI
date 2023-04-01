using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MealMatchAPI.Models;

[Table("Users")]
public class User
{
    [Key]
    public int UserId { get; set; }
    [Required]
    [MaxLength(255)]
    public string Username { get; set; }
    [Required]
    public string PasswordHash { get; set; }
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    [Required]
    [Range(0, 1000)]
    public int Followers { get; set; } = 0;
    [Required]
    [Range(0, 1000)]
    public int Following { get; set; } = 0;
    
    public string? ProfilePictureUrl { get; set; }
    public string? ProfileSettings { get; set; }
    public string? DietLabels { get; set; }
    public string? HealthLabels { get; set; }
    public List<FavoriteRecipe>? FavoriteRecipes { get; set; }
    public List<Like>? LikedRecipes { get; set; }
    public List<Recipe>? OwnedRecipes { get; set; }
}