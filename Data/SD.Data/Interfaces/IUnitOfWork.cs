namespace SD.Data.Interfaces;

public interface IUnitOfWork : IDisposable
{
    void MigrateDb();
    Task Commit();
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
}
