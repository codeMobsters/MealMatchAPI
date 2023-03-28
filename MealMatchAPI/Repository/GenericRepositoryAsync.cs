using System.Linq.Expressions;
using MealMatchAPI.Data;
using MealMatchAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MealMatchAPI.Repository;

public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : class
{
    private readonly ApplicationDbContext _db;
    internal readonly DbSet<T> DbSet;

    public GenericRepositoryAsync(ApplicationDbContext db)
    {
        _db = db;
        this.DbSet = _db.Set<T>();
    }
    public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = DbSet;
        
        if (filter != null)
        {
            query = DbSet.Where(filter);
        }
        
        return await query.ToListAsync();
    }
    
    public virtual async Task<List<T>> GetPagedReponseAsync(int pageNumber, Expression<Func<T, bool>>? filter = null, int pageSize = 20)
    {
        IQueryable<T> query = DbSet;
        
        if (filter != null)
        {
            query = DbSet.Where(filter);
        }
        
        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }
    
    public virtual async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter)
    {
        IQueryable<T> query = DbSet;

        query = query.Where(filter);

        return await query.FirstOrDefaultAsync();
    }
    
    public virtual T GetFirstOrDefault(Expression<Func<T, bool>> filter)
    {
        IQueryable<T> query = DbSet;

        query = query.Where(filter);

        return query.FirstOrDefault();
    }

    public virtual async Task<T> GetByIdAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        return entity;
    }

    public void Update(T entity)
    {
        _db.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        DbSet.Remove(entity);
    }
}