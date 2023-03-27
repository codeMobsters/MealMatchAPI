using MealMatchAPI.Models.APIModels;
using MealMatchAPI.Models.DTOs;

namespace MealMatchAPI.Services
{
  public interface IEdamamApiService
  {
    Task<List<RecipeTransfer>> GetRecipesFromApi();
    Task<List<RecipeTransfer>> GetQueriedRecipesFromApi(RecipeQuery query);
  }
}
