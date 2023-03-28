using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MealMatchAPI.Models;
[Table("Recipes")]
public class Recipe
{
    [Key]
    public int RecipeId { get; set; }
    [Required]
    [MaxLength(60)]
    public string Title { get; set; }
    public int? Like { get; set; }
    public int? Yield { get; set; }
    public double? Calories { get; set; }
    public double TotalTime { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    
    public string Ingredients { get; set; }
    public string? CuisineType { get; set; }
    public string? DietLabels { get; set; }
    public string? DishType { get; set; }
    public string? HealthLabels { get; set; }
    public string? MealType { get; set; }

    [Required]
    public string PictureUrl { get; set; }
    public string? SourceUrl { get; set; }

    [ForeignKey("User")]
    [Required]
    public int UserId { get; set; } // Author of the recipe
    [ValidateNever]
    public virtual User User { get; set; }
    [ValidateNever]
    public List<Comment>? Comments { get; set; }
}

