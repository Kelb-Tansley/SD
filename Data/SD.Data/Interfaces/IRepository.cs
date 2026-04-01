

using System.Linq.Expressions;

namespace SD.Data.Interfaces;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task AddAllAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<T?> FirstOrDefault(Expression<Func<T, bool>> predicate);
}
