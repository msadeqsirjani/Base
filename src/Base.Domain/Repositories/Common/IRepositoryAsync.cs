namespace Base.Domain.Repositories.Common;

public interface IRepositoryAsync<TEntity> : IAsyncDisposable, IRepository<TEntity> where TEntity : BaseEntity, new()
{
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TEntity?> ExecuteScalarAsync(string query, CancellationToken cancellationToken = default, params SqlParameter[] parameters);
    Task ExecuteNonQueryAsync(string query, CancellationToken cancellationToken = default, params SqlParameter[] parameters);
    Task<IEnumerable<TEntity>> ExecuteReaderAsync(string query, CancellationToken cancellationToken = default, params SqlParameter[] parameters);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}