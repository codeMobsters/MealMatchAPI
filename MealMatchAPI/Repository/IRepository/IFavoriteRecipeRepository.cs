using System.Linq.Expressions;
using MealMatchAPI.Models;

namespace MealMatchAPI.Repository.IRepository;

public interface IFavoriteRecipeRepository : IGenericRepositoryAsync<FavoriteRecipe>
{
    Task<List<FavoriteRecipe>> GetAllRecipesFromFollowingAsync(int userId);
}