namespace Liberty.ApplicationShared.Domains.Repositories;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> GetQueryable();

    IQueryable<T> GetQueryableWithAsNoTracking();

    Task<T?> GetByIdAsync(
        long id
    );

    Task<IReadOnlyList<T>> GetAllAsync();

    Task<IReadOnlyList<T>> GetPagedAsync(
        int pageNumber,
        int pageSize
    );

    int SaveChanges();

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    );

    T Add(
        T entity,
        bool autoSave = false
    );

    Task<T> AddAsync(
        T entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    );

    IEnumerable<T> AddRange(
        IEnumerable<T> entities,
        bool autoSave = false
    );

    Task<IEnumerable<T>> AddRangeAsync(
        IEnumerable<T> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    );

    T Update(
        T entity,
        bool autoSave = false
    );

    Task<T> UpdateAsync(
        T entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    );

    IEnumerable<T> UpdateRange(
        IEnumerable<T> entities,
        bool autoSave = false
    );

    Task<IEnumerable<T>> UpdateRangeAsync(
        IEnumerable<T> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    );

    void Delete(
        T entity,
        bool autoSave = false
    );

    Task DeleteAsync(
        T entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    );

    void DeleteRange(
        IEnumerable<T> entities,
        bool autoSave = false
    );

    Task DeleteRangeAsync(
        IEnumerable<T> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    );
}
