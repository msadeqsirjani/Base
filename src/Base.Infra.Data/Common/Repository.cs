namespace Base.Infra.Data.Common;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity, new()
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity> Queryable(bool enableTracking = true)
    {
        return !enableTracking ? DbSet.AsNoTracking() : DbSet;
    }

    public IQueryable<TEntity> Queryable(Expression<Func<TEntity, bool>> predicate, bool enableTracking = true)
    {
        return !enableTracking ? DbSet.AsNoTracking().Where(predicate) : DbSet.Where(predicate);
    }

    public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return DbSet.FirstOrDefault(predicate);
    }

    public void Add(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        DbSet.AddRange(entities);
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        DbSet.UpdateRange(entities);
    }

    public void Attach(TEntity entity)
    {
        DbSet.Attach(entity);
    }

    public void AttacheRange(IEnumerable<TEntity> entities)
    {
        DbSet.AttachRange(entities);
    }

    public void Delete(Guid id)
    {
        var entity = FirstOrDefault(x => x.Id == id);

        if (entity == null) return;

        DbSet.Remove(entity);
    }

    public void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Guid> ids)
    {
        DeleteRange(x => ids.Contains(x.Id));
    }

    public void DeleteRange(Expression<Func<TEntity, bool>> predicate)
    {
        var entities = Queryable(predicate).ToList();

        DbSet.RemoveRange(entities);
    }

    public bool Exists(Expression<Func<TEntity, bool>> predicate)
    {
        return DbSet.Any(predicate);
    }

    public int Count(Expression<Func<TEntity, bool>> predicate)
    {
        return DbSet.Count(predicate);
    }

    public TEntity? ExecuteScalar(string query, params SqlParameter[] parameters)
    {
        var connection = Context.DataBase.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.CommandType = CommandType.Text;
        foreach (var parameter in parameters)
            command.Parameters.Add(parameter);
        if (connection.State.Equals(ConnectionState.Closed))
            connection.Open();
        return command.ExecuteScalar() as TEntity;
    }

    public void ExecuteNonQuery(string query, params SqlParameter[] parameters)
    {
        var connection = Context.DataBase.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.CommandType = CommandType.Text;
        foreach (var parameter in parameters)
            command.Parameters.Add(parameter);
        if (connection.State.Equals(ConnectionState.Closed))
            connection.Open();
        command.ExecuteNonQuery();
    }

    public IEnumerable<TEntity> ExecuteReader(string query, params SqlParameter[] parameters)
    {
        var connection = Context.DataBase.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.CommandType = CommandType.Text;
        foreach (var parameter in parameters)
            command.Parameters.Add(parameter);
        if (connection.State.Equals(ConnectionState.Closed))
            connection.Open();
        var values = command.ExecuteReader();

        List<TEntity> entities = new();

        while (values.Read())
        {
            TEntity entity = new();

            for (var i = 0; i < values.FieldCount; i++)
            {
                var type = entity.GetType();
                var property = type.GetProperty(values.GetName(i));
                property?.SetValue(entity, values.GetValue(i), null);
            }

            entities.Add(entity);
        }

        return entities;
    }

    public void SaveChanges()
    {
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}