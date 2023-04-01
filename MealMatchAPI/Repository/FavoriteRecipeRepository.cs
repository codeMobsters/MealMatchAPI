using System.Linq.Expressions;
using MealMatchAPI.Data;
using MealMatchAPI.Models;
using MealMatchAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MealMatchAPI.Repository;

public class FavoriteRecipeRepository : GenericRepositoryAsync<FavoriteRecipe>, IFavoriteRecipeRepository
{
    private readonly ApplicationDbContext _db;

    public FavoriteRecipeRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public override async Task<List<FavoriteRecipe>> GetAllAsync(Expression<Func<FavoriteRecipe, bool>>? filter = null)
    {
        IQueryable<FavoriteRecipe> query = _db.FavoriteRecipes;
        if (filter != null)
        {
            query = _db.FavoriteRecipes.Where(filter);
        }
        return await query
            .Include(recipe => recipe.Recipe)
            .Include(recipe => recipe.Recipe.Comments)!
            .ThenInclude(comment => comment.User)
            .Include(recipe => recipe.Recipe.User)
            .Include(recipe => recipe.User)
            .AsNoTracking()
            .ToListAsync();
    }
    
    public override async Task<List<FavoriteRecipe>> GetPagedReponseAsync(int pageNumber, Expression<Func<FavoriteRecipe, bool>>? filter = null, int pageSize = 20)
    {
        IQueryable<FavoriteRecipe> query = _db.FavoriteRecipes;
        if (filter != null)
        {
            query = _db.FavoriteRecipes.Where(filter);
        }
        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(recipe => recipe.Recipe)
            .Include(recipe => recipe.Recipe.Comments)!
            .ThenInclude(comment => comment.User)
            .Include(recipe => recipe.Recipe.User)
            .Include(recipe => recipe.User)
            .AsNoTracking()
            .ToListAsync();
    }
    
    public override async Task<FavoriteRecipe?> GetByIdAsync(int id)
    {
        return await _db.FavoriteRecipes
            .Include(recipe => recipe.Recipe)
            .Include(recipe => recipe.Recipe.Comments)!
            .ThenInclude(comment => comment.User)
            .Include(recipe => recipe.Recipe.User)
            .Include(recipe => recipe.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(recipe => recipe.RecipeId == id);
    }
    
    public override async Task<FavoriteRecipe?> GetFirstOrDefaultAsync(Expression<Func<FavoriteRecipe, bool>> filter)
    {
        IQueryable<FavoriteRecipe> query = _db.FavoriteRecipes;

        query = query.Where(filter);

        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
}