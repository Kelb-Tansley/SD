using SD.Data.Interfaces;

namespace SD.Data.Repository;

public class UnitOfWork(StructuralDesignContext context) : IUnitOfWork
{
    private readonly StructuralDesignContext _context = context;
    private readonly Dictionary<Type, object> _repositories = [];

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity);
        if (_repositories.TryGetValue(type, out object? value))
            return (IRepository<TEntity>)value!;

        var repository = new Repository<TEntity>(_context);
        _repositories.Add(type, repository);
        return repository;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}