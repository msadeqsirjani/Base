namespace Base.Application.Services.Common;

public interface IService<TEntity> where TEntity : BaseEntity, new()
{
    IEnumerable<TEntity> Select();
    IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> predicate);
    TEntity? FirstOrDefault(Guid id);
    TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Attach(TEntity entity);
    void AttacheRange(IEnumerable<TEntity> entities);
    void Delete(TEntity entity);
    void Delete(Guid id);
    void DeleteRange(IEnumerable<Guid> ids);
    void DeleteRange(Expression<Func<TEntity, bool>> predicate);
    bool Exists(Expression<Func<TEntity, bool>> predicate);
}

public class Service<TEntity> : IService<TEntity> where TEntity : BaseEntity, new()
{
    protected readonly IRepositoryAsync<TEntity> Repository;

    public Service(IRepositoryAsync<TEntity> repository)
    {
        Repository = repository;
    }

    public virtual IEnumerable<TEntity> Select() => Repository.Queryable(false);

    public virtual IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> predicate) =>
        Repository.Queryable(predicate, false);

    public virtual TEntity? FirstOrDefault(Guid id) => Repository.FirstOrDefault(x => x.Id == id);

    public virtual TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate) => Repository.FirstOrDefault(predicate);

    public virtual void Add(TEntity entity) => Repository.Add(entity);

    public virtual void AddRange(IEnumerable<TEntity> entities) => Repository.AddRange(entities);

    public virtual void Update(TEntity entity) => Repository.Update(entity);

    public virtual void UpdateRange(IEnumerable<TEntity> entities) => Repository.UpdateRange(entities);

    public virtual void Attach(TEntity entity) => Repository.Attach(entity);

    public virtual void AttacheRange(IEnumerable<TEntity> entities) => Repository.AttacheRange(entities);

    public virtual void Delete(TEntity entity) => Repository.Delete(entity);

    public virtual void Delete(Guid id) => Repository.Delete(id);

    public virtual void DeleteRange(IEnumerable<Guid> ids) => Repository.DeleteRange(ids);

    public virtual void DeleteRange(Expression<Func<TEntity, bool>> predicate) => Repository.DeleteRange(predicate);

    public virtual bool Exists(Expression<Func<TEntity, bool>> predicate) => Repository.Exists(predicate);
}