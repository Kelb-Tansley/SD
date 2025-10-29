namespace SD.Data.Interfaces;
public interface IUnitOfWork : IDisposable
{
    void Commit();
    void Rollback();
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
}
