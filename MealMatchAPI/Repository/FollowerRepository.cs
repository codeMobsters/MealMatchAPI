using System.Linq.Expressions;
using MealMatchAPI.Data;
using MealMatchAPI.Models;
using MealMatchAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MealMatchAPI.Repository;

public class FollowerRepository : GenericRepositoryAsync<Follower>, IFollowerRepository
{
    private readonly ApplicationDbContext _db;

    public FollowerRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public override async Task<List<Follower>> GetAllAsync(Expression<Func<Follower, bool>>? filter = null)
    {
        IQueryable<Follower> query = _db.Followers
            .Include(user => user.FollowingUser)
            .Include(user => user.FollowedUser);

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query
            .AsNoTracking()
            .ToListAsync();
    }
}