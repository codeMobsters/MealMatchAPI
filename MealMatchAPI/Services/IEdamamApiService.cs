using MealMatchAPI.Models.DTOs;

namespace MealMatchAPI.Services
{
  public interface IEdamamApiService
  {
    Task<RecipeTransfer> GetRecipesFromApi();
  }
}
