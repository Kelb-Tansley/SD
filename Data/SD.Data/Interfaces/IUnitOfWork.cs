namespace SD.Data.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task Commit();
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
}