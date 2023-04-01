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
}