using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MealMatchAPI.Models;

[Table("Followers")]
public class Follower
{
    [Key]
    public int FollowerId { get; set; }
    [ForeignKey("FollowingUser")]
    [Required]
    public int FollowingUserId { get; set; } // This user follows the other
    [ValidateNever]
    public virtual User FollowingUser { get; set; }
    
    [ForeignKey("FollowedUser")]
    [Required]
    public int FollowedUserId { get; set; } // 
    [ValidateNever]
    public virtual User FollowedUser { get; set; }
}