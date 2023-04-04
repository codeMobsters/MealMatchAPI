using System.Linq.Expressions;
using MealMatchAPI.Data;
using MealMatchAPI.Models;
using MealMatchAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
            .Include(recipe => recipe.Recipe.Likes)
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

    public async Task<List<FavoriteRecipe>> GetAllRecipesFromFollowingAsync(int userId)
    {
        var following = _db.Followers.Where(follower => follower.FollowingUserId == userId)
            .Select(follower => follower.FollowedUserId).ToList();

        var user = _db.Users.Find(userId);
        var healthLabels = !string.IsNullOrEmpty(user?.HealthLabels) ? user?.HealthLabels?.Split("<//>").ToList() : null;
        var dietLabels = !string.IsNullOrEmpty(user?.DietLabels) ? user?.DietLabels?.Split("<//>").ToList() : null;
        
        IQueryable<FavoriteRecipe> query = _db.FavoriteRecipes.Where(recipe => following.Contains(recipe.UserId))
            .Include(recipe => recipe.Recipe);

        if (healthLabels?.Count > 0)
        {
            healthLabels.ForEach(label => query = query.Where(recipe =>
                recipe.Recipe.HealthLabels != null && recipe.Recipe.HealthLabels.Contains(label)));
        }
        
        if (dietLabels?.Count > 0)
        {
            dietLabels.ForEach(label => query = query.Where(recipe =>
                recipe.Recipe.DietLabels != null && recipe.Recipe.DietLabels.Contains(label)));
        }
        
        return await query
            .Include(recipe => recipe.Recipe.Comments)!
            .ThenInclude(comment => comment.User)
            .Include(recipe => recipe.Recipe.Likes)
            .Include(recipe => recipe.Recipe.User)
            .Include(recipe => recipe.User)
            .OrderByDescending(recipe => recipe.FavoriteRecipeId)
            .AsNoTracking()
            .ToListAsync();
    }
}