using MealMatchAPI.Data;
using MealMatchAPI.Repository.IRepository;

namespace MealMatchAPI.Repository;

public class Repositories : IRepositories
{
    private ApplicationDbContext _db;
    public IRecipeRepository Recipe { get; }
    public IFavoriteRecipeRepository FavoriteRecipe { get; }
    public IUserRepository User { get; }
    public ICommentRepository Comment { get; }
    public ILikeRepository Like { get; }
    public IFollowerRepository Follower { get; }

    public Repositories(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        Comment = new CommentRepository(_db);
        User = new UserRepository(_db, configuration);
        Recipe = new RecipeRepository(_db);
        FavoriteRecipe = new FavoriteRecipeRepository(_db);
        Like = new LikeRepository(_db);
        Follower = new FollowerRepository(_db);
    }
    public async Task Save()
    {
        await _db.SaveChangesAsync();
    }
}