namespace Base.Domain.Repositories.Common;

public interface IRepository<TEntity> : IDisposable where TEntity : BaseEntity, new()
{
    IQueryable<TEntity> Queryable(bool enableTracking = true);
    IQueryable<TEntity> Queryable(Expression<Func<TEntity, bool>> predicate, bool enableTracking = true);
    TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Attach(TEntity entity);
    void AttacheRange(IEnumerable<TEntity> entities);
    void Delete(Guid id);
    void Delete(TEntity entity);
    void DeleteRange(IEnumerable<Guid> ids);
    void DeleteRange(Expression<Func<TEntity, bool>> predicate);
    bool Exists(Expression<Func<TEntity, bool>> predicate);
    int Count(Expression<Func<TEntity, bool>> predicate);
    TEntity? ExecuteScalar(string query, params SqlParameter[] parameters);
    void ExecuteNonQuery(string query, params SqlParameter[] parameters);
    IEnumerable<TEntity> ExecuteReader(string query, params SqlParameter[] parameters);
    void SaveChanges();
}