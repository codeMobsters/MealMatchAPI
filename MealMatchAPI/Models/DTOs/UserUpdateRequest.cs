namespace MealMatchAPI.Models.DTOs;

public class UserUpdateRequest
{
    public string? Name { get; set; }
    
    public IFormFile? ProfilePicture { get; set; }
    public List<string>? ProfileSettings { get; set; }
    public List<string>? DietLabels { get; set; }
    public List<string>? HealthLabels { get; set; }
}