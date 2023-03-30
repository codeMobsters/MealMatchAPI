using System.Linq.Expressions;

namespace MealMatchAPI.Repository.IRepository;

public interface IGenericRepositoryAsync<T> where T : class
{
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
    Task<List<T>> GetPagedReponseAsync(int pageNumber, Expression<Func<T, bool>>? filter = null, int pageSize = 20);
    Task<T> GetByIdAsync(int id);
    Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter);
    T GetFirstOrDefault(Expression<Func<T, bool>> filter);
    Task<T> AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> Exists(Expression<Func<T, bool>> filter);
}