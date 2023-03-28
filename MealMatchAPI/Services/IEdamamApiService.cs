using MealMatchAPI.Models.APIModels;
using MealMatchAPI.Models.DTOs;

namespace MealMatchAPI.Services
{
  public interface IEdamamApiService
  {
    Task<EdamamResponse> GetRecipesFromApi();
    Task<EdamamResponse> GetQueriedRecipesFromApi(RecipeQuery query);
    Task<EdamamResponse> GetNextRecipesFromApi(string nextUrl);
  }
}
