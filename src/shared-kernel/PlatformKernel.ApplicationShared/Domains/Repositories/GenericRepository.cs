using Microsoft.EntityFrameworkCore;

namespace PlatformKernel.ApplicationShared.Domains.Repositories;

public class GenericRepository<T>(
    DbContext dbContext
) : IGenericRepository<T>
    where T : class
{
    public virtual IQueryable<T> GetQueryable()
    {
        return dbContext.Set<T>().AsQueryable();
    }

    public virtual IQueryable<T> GetQueryableWithAsNoTracking()
    {
        return GetQueryable().AsNoTracking();
    }

    public virtual IQueryable<T> GetQueryableWithAsNoTracking(
        DbContext appDbContext
    )
    {
        return appDbContext.Set<T>().AsNoTracking();
    }

    public virtual async Task<T?> GetByIdAsync(
        long id
    )
    {
        return await dbContext.Set<T>().FindAsync(id);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await dbContext
            .Set<T>()
            .AsNoTracking()
            .ToListAsync();
    }

    public virtual async Task<IReadOnlyList<T>> GetPagedAsync(
        int pageNumber,
        int pageSize
    )
    {
        return await dbContext
            .Set<T>()
            .Skip(Math.Max(0, pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public int SaveChanges()
    {
        return dbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }

    public T Add(
        T entity,
        bool autoSave = false
    )
    {
        dbContext.Set<T>().Add(entity);
        if (autoSave) { SaveChanges(); }

        return entity;
    }

    public async Task<T> AddAsync(
        T entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext.Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
        if (autoSave) { await SaveChangesAsync(cancellationToken); }

        return entity;
    }

    public IEnumerable<T> AddRange(
        IEnumerable<T> entities,
        bool autoSave = false
    )
    {
        var data = entities as T[] ?? [.. entities];
        dbContext.Set<T>().AddRange(data);
        if (autoSave) { SaveChanges(); }

        return data;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(
        IEnumerable<T> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = entities as T[] ?? [.. entities];
        await dbContext.Set<T>().AddRangeAsync(data, cancellationToken).ConfigureAwait(false);
        if (autoSave) { await SaveChangesAsync(cancellationToken); }

        return data;
    }

    public T Update(
        T entity,
        bool autoSave = false
    )
    {
        dbContext.Set<T>().Update(entity);
        if (autoSave) { SaveChanges(); }

        return entity;
    }

    public async Task<T> UpdateAsync(
        T entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    )
    {
        dbContext.Set<T>().Update(entity);
        if (autoSave) { await SaveChangesAsync(cancellationToken); }

        return entity;
    }

    public IEnumerable<T> UpdateRange(
        IEnumerable<T> entities,
        bool autoSave = false
    )
    {
        var data = entities as T[] ?? [.. entities];
        dbContext.Set<T>().UpdateRange(data);
        if (autoSave) { SaveChanges(); }

        return data;
    }

    public async Task<IEnumerable<T>> UpdateRangeAsync(
        IEnumerable<T> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = entities as T[] ?? [.. entities];
        dbContext.Set<T>().UpdateRange(data);
        if (autoSave) { await SaveChangesAsync(cancellationToken); }

        return data;
    }

    public void Delete(
        T entity,
        bool autoSave = false
    )
    {
        dbContext.Set<T>().Remove(entity);
        if (autoSave) { SaveChanges(); }
    }

    public async Task DeleteAsync(
        T entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    )
    {
        dbContext.Set<T>().Remove(entity);
        if (autoSave) { await SaveChangesAsync(cancellationToken); }
    }

    public void DeleteRange(
        IEnumerable<T> entities,
        bool autoSave = false
    )
    {
        dbContext.Set<T>().RemoveRange(entities);
        if (autoSave) { SaveChanges(); }
    }

    public async Task DeleteRangeAsync(
        IEnumerable<T> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    )
    {
        dbContext.Set<T>().RemoveRange(entities);
        if (autoSave) { await SaveChangesAsync(cancellationToken); }
    }
}
