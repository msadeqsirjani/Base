namespace Base.Domain.Repositories.Common;

public interface IUnitOfWork : IDisposable
{
    void SaveChanges();
    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();
}