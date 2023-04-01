namespace MealMatchAPI.Models.DTOs;
public class CommentTransfer
{

    public int CommentId { get; set; }
    public string CommentText { get; set; }
    public DateTime CommentAt { get; set; }
    public int RecipeId { get; set; } // Recipe to be commented at
    public string UserName { get; set; }
    public string? UserProfilePictureUrl { get; set; }

}

