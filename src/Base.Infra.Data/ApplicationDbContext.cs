namespace Base.Infra.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        Base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override int SaveChanges()
    {
        OnBeforeSaveChanges();

        return Base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaveChanges();

        return Base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        OnBeforeSaveChanges();

        return Base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new())
    {
        OnBeforeSaveChanges();

        return Base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();

        var records = ChangeTracker.Entries();
        var entityEntries = records.ToList();

        var addedEntries = entityEntries.Where(x => x.State == EntityState.Added)
            .Select(x => x.Entity)
            .OfType<BaseEntity>()
            .ToList();

        var updatedEntries = entityEntries.Where(x => x.State == EntityState.Modified)
            .Select(x => x.Entity)
            .OfType<BaseEntity>()
            .ToList();

        var now = DateTime.UtcNow;

        addedEntries.ForEach(x =>
        {
            x.CreatedAt = now;
            x.ModifiedAt = now;
        });

        updatedEntries.ForEach(x =>
        {
            x.ModifiedAt = now;
        });
    }
}