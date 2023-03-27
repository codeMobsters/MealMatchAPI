using MealMatchAPI.Data;
using MealMatchAPI.Models;
using MealMatchAPI.Repository.IRepository;

namespace MealMatchAPI.Repository;

public class RecipeRepository : GenericRepositoryAsync<Recipe>, IRecipeRepository
{
    private readonly ApplicationDbContext _db;

    public RecipeRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
}