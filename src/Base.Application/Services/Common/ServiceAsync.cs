namespace Base.Application.Services.Common;

public interface IServiceAsync<TEntity> : IService<TEntity> where TEntity : BaseEntity, new()
{
    Task<TEntity?> FirstOrDefaultAsync(Guid id, CancellationToken cancellationToken = new());
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new());
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = new());
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = new());
    Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = new());
    Task DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new());
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new());
}

public class ServiceAsync<TEntity> : Service<TEntity>, IServiceAsync<TEntity> where TEntity : BaseEntity, new()
{
    public ServiceAsync(IRepositoryAsync<TEntity> repository) : base(repository)
    {
    }

    public virtual Task<TEntity?> FirstOrDefaultAsync(Guid id, CancellationToken cancellationToken = new()) => Repository.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public virtual Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new()) =>
        Repository.FirstOrDefaultAsync(predicate, cancellationToken);

    public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = new()) => Repository.AddAsync(entity, cancellationToken);

    public virtual Task AddRangeAsync(IEnumerable<TEntity> entities) => Repository.AddRangAsync(entities);

    public virtual Task DeleteAsync(Guid id, CancellationToken cancellationToken = new()) =>
        Repository.DeleteAsync(id, cancellationToken);

    public virtual Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = new()) =>
        Repository.DeleteRangeAsync(ids, cancellationToken);

    public virtual Task DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = new()) => Repository.DeleteRangeAsync(predicate, cancellationToken);

    public virtual Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = new()) => Repository.ExistsAsync(predicate, cancellationToken);
}