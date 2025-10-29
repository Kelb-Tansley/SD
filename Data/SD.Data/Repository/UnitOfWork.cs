using SD.Data.Interfaces;

namespace SD.Data.Repository;
public class UnitOfWork : IUnitOfWork
{
    private readonly StructuralDesignContext _context;
    private readonly Dictionary<Type, object> _repositories = [];

    public UnitOfWork(StructuralDesignContext context)
    {
        _context = context;
    }

    public void Commit()
    {
        _context.SaveChanges();
    }

    public void Rollback()
    {
        // Rollback changes if needed
    }

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        if (_repositories.ContainsKey(typeof(TEntity)))
            return (IRepository<TEntity>)_repositories[typeof(TEntity)];

        var repository = new Repository<TEntity>(_context);
        _repositories.Add(typeof(TEntity), repository);
        return repository;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
