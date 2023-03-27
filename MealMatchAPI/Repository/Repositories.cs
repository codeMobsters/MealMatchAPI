using MealMatchAPI.Data;
using MealMatchAPI.Repository.IRepository;

namespace MealMatchAPI.Repository;

public class Repositories : IRepositories
{
    private ApplicationDbContext _db;
    public IRecipeRepository Recipe { get; }
    public IUserRepository User { get; }
    public ICommentRepository Comment { get; }

    public Repositories(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        Comment = new CommentRepository(_db);
        User = new UserRepository(_db, configuration);
        Recipe = new RecipeRepository(_db);
    }
    public async Task Save()
    {
        await _db.SaveChangesAsync();
    }
}