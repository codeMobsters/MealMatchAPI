using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MealMatchAPI.Models;
[Table("Likes")]
public class Like
{
    [Key]
    public int LikeId { get; set; }
    [Required]
    [ForeignKey("Recipe")]
    public int RecipeId { get; set; } // The recipe itself
    [ValidateNever]
    public virtual Recipe Recipe { get; set; }
    
    [ForeignKey("User")]
    [Required]
    public int UserId { get; set; } // Person who liked the recipe
    [ValidateNever]
    public virtual User User { get; set; }
}