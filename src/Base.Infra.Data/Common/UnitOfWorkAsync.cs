namespace Base.Infra.Data.Common;

public class UnitOfWorkAsync : UnitOfWork, IUnitOfWorkAsync
{
    public UnitOfWorkAsync(ApplicationDbContext context) : base(context)
    {
    }

    public ValueTask DisposeAsync()
    {
        return Context.DisposeAsync();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Context.SaveChangesAsync(cancellationToken);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.DataBase.BeginTransactionAsync(cancellationToken);
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.DataBase.CommitTransactionAsync(cancellationToken);
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.DataBase.RollbackTransactionAsync(cancellationToken);
    }
}