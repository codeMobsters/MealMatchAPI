using System.Linq.Expressions;
using MealMatchAPI.Data;
using MealMatchAPI.Models;
using MealMatchAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MealMatchAPI.Repository;

public class CommentRepository : GenericRepositoryAsync<Comment>, ICommentRepository
{
    private readonly ApplicationDbContext _db;

    public CommentRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
    
    public override async Task<List<Comment>> GetAllAsync(Expression<Func<Comment, bool>>? filter = null)
    {
        IQueryable<Comment> query = _db.Comments;
        if (filter != null)
        {
            query = _db.Comments.Where(filter);
        }
        return await query
            .Include(recipe => recipe.User)
            .ToListAsync();
    }
    
    public override async Task<Comment> GetByIdAsync(int id)
    {
        return (await _db.Comments
            .Include(comment => comment.User)
            .FirstOrDefaultAsync(comment => comment.CommentId == id))!;
    }
    
    public override async Task<Comment> GetFirstOrDefaultAsync(Expression<Func<Comment, bool>> filter)
    {
        IQueryable<Comment> query = _db.Comments;

        query = query.Where(filter);

        return (await query
            .Include(comment => comment.User)
            .FirstOrDefaultAsync())!;
    }
}