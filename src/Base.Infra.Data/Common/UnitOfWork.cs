namespace Base.Infra.Data.Common;

public class UnitOfWork : IUnitOfWork
{
    protected readonly ApplicationDbContext Context;

    public UnitOfWork(ApplicationDbContext context)
    {
        Context = context;
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    public void SaveChanges()
    {
        Context.SaveChanges();
    }

    public void BeginTransaction()
    {
        Context.DataBase.BeginTransaction();
    }

    public void CommitTransaction()
    {
        Context.DataBase.CommitTransaction();
    }

    public void RollbackTransaction()
    {
        Context.DataBase.RollbackTransaction();
    }
}