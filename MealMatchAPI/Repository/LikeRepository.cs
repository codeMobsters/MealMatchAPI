using System.Linq.Expressions;
using MealMatchAPI.Data;
using MealMatchAPI.Models;
using MealMatchAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MealMatchAPI.Repository;

public class LikeRepository : GenericRepositoryAsync<Like>, ILikeRepository
{
    private readonly ApplicationDbContext _db;

    public LikeRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
}