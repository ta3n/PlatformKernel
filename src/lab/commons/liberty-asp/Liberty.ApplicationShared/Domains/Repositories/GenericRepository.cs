using Microsoft.EntityFrameworkCore;

namespace Liberty.ApplicationShared.Domains.Repositories;

public class GenericRepository<T>(
    DbContext dbContext
) : IGenericRepository<T>
    where T : class
{
    protected readonly DbContext DbContext = dbContext;

    public IQueryable<T> GetQueryable()
    {
        return DbContext.Set<T>().AsQueryable();
    }

    public IQueryable<T> GetQueryableWithAsNoTracking()
    {
        return GetQueryable().AsNoTracking();
    }

    public virtual async Task<T?> GetByIdAsync(
        long id
    )
    {
        return await DbContext.Set<T>().FindAsync(id);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await DbContext
            .Set<T>()
            .AsNoTracking()
            .ToListAsync();
    }

    public virtual async Task<IReadOnlyList<T>> GetPagedAsync(
        int pageNumber,
        int pageSize
    )
    {
        return await DbContext
            .Set<T>()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public int SaveChanges()
    {
        return DbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await DbContext.SaveChangesAsync(cancellationToken);
    }

    public T Add(
        T entity,
        bool autoSave = false
    )
    {
        DbContext.Set<T>().Add(entity);
        if (autoSave) { SaveChanges(); }

        return entity;
    }

    public async Task<T> AddAsync(
        T entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    )
    {
        await DbContext.Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
        if (autoSave) { await SaveChangesAsync(cancellationToken); }

        return entity;
    }

    public IEnumerable<T> AddRange(
        IEnumerable<T> entities,
        bool autoSave = false
    )
    {
        var data = entities as T[] ?? entities.ToArray();
        DbContext.Set<T>().AddRange(data);
        if (autoSave) { SaveChanges(); }

        return data;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(
        IEnumerable<T> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = entities as T[] ?? entities.ToArray();
        await DbContext.Set<T>().AddRangeAsync(data, cancellationToken).ConfigureAwait(false);
        if (autoSave) { await SaveChangesAsync(cancellationToken); }

        return data;
    }

    public T Update(
        T entity,
        bool autoSave = false
    )
    {
        DbContext.Set<T>().Update(entity);
        if (autoSave) { SaveChanges(); }

        return entity;
    }

    public async Task<T> UpdateAsync(
        T entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    )
    {
        DbContext.Set<T>().Update(entity);
        if (autoSave) { await SaveChangesAsync(cancellationToken); }

        return entity;
    }

    public IEnumerable<T> UpdateRange(
        IEnumerable<T> entities,
        bool autoSave = false
    )
    {
        var data = entities as T[] ?? entities.ToArray();
        DbContext.Set<T>().UpdateRange(data);
        if (autoSave) { SaveChanges(); }

        return data;
    }

    public async Task<IEnumerable<T>> UpdateRangeAsync(
        IEnumerable<T> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = entities as T[] ?? entities.ToArray();
        DbContext.Set<T>().UpdateRange(data);
        if (autoSave) { await SaveChangesAsync(cancellationToken); }

        return data;
    }

    public void Delete(
        T entity,
        bool autoSave = false
    )
    {
        DbContext.Set<T>().Remove(entity);
        if (autoSave) { SaveChanges(); }
    }

    public async Task DeleteAsync(
        T entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    )
    {
        DbContext.Set<T>().Remove(entity);
        if (autoSave) { await SaveChangesAsync(cancellationToken); }
    }

    public void DeleteRange(
        IEnumerable<T> entities,
        bool autoSave = false
    )
    {
        DbContext.Set<T>().RemoveRange(entities);
        if (autoSave) { SaveChanges(); }
    }

    public async Task DeleteRangeAsync(
        IEnumerable<T> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    )
    {
        DbContext.Set<T>().RemoveRange(entities);
        if (autoSave) { await SaveChangesAsync(cancellationToken); }
    }
}
