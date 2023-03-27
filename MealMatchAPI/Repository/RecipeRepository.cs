using System.Linq.Expressions;
using MealMatchAPI.Data;
using MealMatchAPI.Models;
using MealMatchAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MealMatchAPI.Repository;

public class RecipeRepository : GenericRepositoryAsync<Recipe>, IRecipeRepository
{
    private readonly ApplicationDbContext _db;

    public RecipeRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public override async Task<List<Recipe>> GetAllAsync(Expression<Func<Recipe, bool>>? filter = null)
    {
        IQueryable<Recipe> query = _db.Recipes;
        if (filter != null)
        {
            query = _db.Recipes.Where(filter);
        }
        return await query.Include(recipe => recipe.Comments).ToListAsync();
    }
    
    public override async Task<Recipe?> GetByIdAsync(int id)
    {
        return await _db.Recipes
            .Include(recipe => recipe.Comments)
            .FirstOrDefaultAsync(recipe => recipe.RecipeId == id);
    }
    
    public override async Task<Recipe?> GetFirstOrDefaultAsync(Expression<Func<Recipe, bool>> filter)
    {
        IQueryable<Recipe> query = _db.Recipes;

        query = query.Where(filter);

        return await query.Include(recipe => recipe.Comments).FirstOrDefaultAsync();
    }

}