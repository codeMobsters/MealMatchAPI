using MealMatchAPI.Data;
using MealMatchAPI.Models;
using MealMatchAPI.Repository.IRepository;

namespace MealMatchAPI.Repository;

public class CommentRepository : GenericRepositoryAsync<Comment>, ICommentRepository
{
    private readonly ApplicationDbContext _db;

    public CommentRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
}