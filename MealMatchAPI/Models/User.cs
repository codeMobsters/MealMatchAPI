using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    
    public string? ProfilePictureUrl { get; set; }
    public string? ProfileSettings { get; set; }
    public string? DietLabels { get; set; }
    public string? HealthLabels { get; set; }
}