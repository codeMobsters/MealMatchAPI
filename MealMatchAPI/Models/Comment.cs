using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MealMatchAPI.Models;
[Table("Comments")]
public class Comment
{
    [Key]
    public int CommentId { get; set; }
    [Required]
    [MaxLength(2000)]
    public string CommentText { get; set; }
    [Required]
    public DateTime CommentAt { get; set; }
    
    [ForeignKey("User")]
    [Required]
    public int UserId { get; set; } // Author of the comment
    [ValidateNever]
    [JsonIgnore]
    public User User { get; set; }
    
    [ForeignKey("Recipe")]
    [Required]
    public int RecipeId { get; set; } // Recipe to be commented at
    [ValidateNever]
    [JsonIgnore]
    public Recipe Recipe { get; set; }
}

