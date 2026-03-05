using AutoFilterer.Extensions;
using AutoFilterer.Types;
using Liberty.Application.Domains.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;
using System.Linq.Expressions;

namespace Liberty.Application.Domains.Repositories;

public interface IGenericRepository<T> where T : class
{
    //IUnitOfWork UnitOfWork { get; }
    Task<T?> GetByIdAsync(long id);

    Task<IReadOnlyList<T>> GetAllAsync();

    Task<IReadOnlyList<T>> GetPagedReponseAsync(int pageNumber, int pageSize);

    IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class;

    IQueryable<TEntity> GetQueryable<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

    List<TEntity> GetMultiple<TEntity>(bool asNoTracking) where TEntity : class;

    Task<List<TEntity>> GetMultipleAsync<TEntity>(bool asNoTracking, CancellationToken cancellationToken = default) where TEntity : class;

    List<TProjected> GetMultiple<TEntity, TProjected>(bool asNoTracking, Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class;

    Task<List<TProjected>> GetMultipleAsync<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, TProjected>> projectExpression,
        CancellationToken cancellationToken = default) where TEntity : class;

    List<TEntity> GetMultiple<TEntity>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression) where TEntity : class;

    Task<List<TEntity>> GetMultipleAsync<TEntity>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        CancellationToken cancellationToken = default) where TEntity : class;

    List<TProjected> GetMultiple<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class;

    Task<List<TProjected>> GetMultipleAsync<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, TProjected>> projectExpression,
        CancellationToken cancellationToken = default) where TEntity : class;

    List<TEntity> GetMultiple<TEntity>(bool asNoTracking,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression) where TEntity : class;

    Task<List<TEntity>> GetMultipleAsync<TEntity>(bool asNoTracking,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default) where TEntity : class;

    List<TEntity> GetMultiple<TEntity>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression) where TEntity : class;

    Task<List<TEntity>> GetMultipleAsync<TEntity>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default) where TEntity : class;

    List<TProjected> GetMultiple<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class;

    Task<List<TProjected>> GetMultipleAsync<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        Expression<Func<TEntity, TProjected>> projectExpression,
        CancellationToken cancellationToken = default) where TEntity : class;

    List<TEntity> GetMultiple<TEntity, TFilter>(bool asNoTracking, TFilter filter
        ) where TEntity : class where TFilter : FilterBase;

    Task<List<TEntity>> GetMultipleAsync<TEntity, TFilter>(bool asNoTracking,
        TFilter filter,
        CancellationToken cancellationToken = default
        ) where TEntity : class where TFilter : FilterBase;

    List<TEntity> GetMultiple<TEntity, TFilter>(bool asNoTracking,
        TFilter filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression
        ) where TEntity : class where TFilter : FilterBase;

    Task<List<TEntity>> GetMultipleAsync<TEntity, TFilter>(bool asNoTracking,
        TFilter filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default
        ) where TEntity : class where TFilter : FilterBase;

    List<TProjected> GetMultiple<TEntity, TFilter, TProjected>(bool asNoTracking,
        TFilter filter,
        Expression<Func<TEntity, TProjected>> projectExpression
        ) where TEntity : class where TFilter : FilterBase;

    Task<List<TProjected>> GetMultipleAsync<TEntity, TFilter, TProjected>(bool asNoTracking,
        TFilter filter,
        Expression<Func<TEntity, TProjected>> projectExpression,
        CancellationToken cancellationToken = default
        ) where TEntity : class where TFilter : FilterBase;

    List<TProjected> GetMultiple<TEntity, TFilter, TProjected>(bool asNoTracking,
        TFilter filter,
        Expression<Func<TEntity, TProjected>> projectExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression
        ) where TEntity : class where TFilter : FilterBase;

    Task<List<TProjected>> GetMultipleAsync<TEntity, TFilter, TProjected>(bool asNoTracking,
        TFilter filter,
        Expression<Func<TEntity, TProjected>> projectExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default
        ) where TEntity : class where TFilter : FilterBase;

    TEntity? GetSingle<TEntity>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression) where TEntity : class;

    Task<TEntity?> GetSingleAsync<TEntity>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        CancellationToken cancellationToken = default) where TEntity : class;

    TEntity? GetSingle<TEntity>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression
        ) where TEntity : class;

    Task<TEntity?> GetSingleAsync<TEntity>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default) where TEntity : class;

    TProjected? GetSingle<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class;

    Task<TProjected?> GetSingleAsync<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, TProjected>> projectExpression,
        CancellationToken cancellationToken = default) where TEntity : class;

    TProjected? GetSingle<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, TProjected>> projectExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression
        ) where TEntity : class;

    Task<TProjected?> GetSingleAsync<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, TProjected>> projectExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default) where TEntity : class;

    TEntity? GetSingle<TEntity, TFilter>(bool asNoTracking, TFilter filter) where TEntity : class where TFilter : FilterBase;

    Task<TEntity?> GetSingleAsync<TEntity, TFilter>(bool asNoTracking,
        TFilter filter,
        CancellationToken cancellationToken = default) where TEntity : class where TFilter : FilterBase;

    TEntity? GetSingle<TEntity, TFilter>(bool asNoTracking,
        TFilter filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression
        ) where TEntity : class where TFilter : FilterBase;

    Task<TEntity?> GetSingleAsync<TEntity, TFilter>(bool asNoTracking,
        TFilter filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default
        ) where TEntity : class where TFilter : FilterBase;

    TProjected? GetSingle<TEntity, TProjected, TFilter>(bool asNoTracking,
        TFilter filter,
        Expression<Func<TEntity, TProjected>> projectExpression
        ) where TEntity : class where TFilter : FilterBase;

    Task<TProjected?> GetSingleAsync<TEntity, TProjected, TFilter>(bool asNoTracking,
        TFilter filter,
        Expression<Func<TEntity, TProjected>> projectExpression,
        CancellationToken cancellationToken = default
        ) where TEntity : class where TFilter : FilterBase;

    TProjected? GetSingle<TEntity, TProjected, TFilter>(bool asNoTracking,
        TFilter filter,
        Expression<Func<TEntity, TProjected>> projectExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression
        ) where TEntity : class where TFilter : FilterBase;

    Task<TProjected?> GetSingleAsync<TEntity, TProjected, TFilter>(bool asNoTracking,
        TFilter filter,
        Expression<Func<TEntity, TProjected>> projectExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default
        ) where TEntity : class where TFilter : FilterBase;


    TEntity? GetById<TEntity>(object id) where TEntity : class;

    Task<TEntity?> GetByIdAsync<TEntity>(bool asNoTracking, object id, CancellationToken cancellationToken = default) where TEntity : class;

    TEntity? GetById<TEntity>(bool asNoTracking,
        object id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression
        ) where TEntity : class;

    Task<TEntity?> GetByIdAsync<TEntity>(bool asNoTracking,
        object id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default
        ) where TEntity : class;


    TProjected? GetById<TEntity, TProjected>(bool asNoTracking,
        object id,
        Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class;

    Task<TProjected?> GetByIdAsync<TEntity, TProjected>(bool asNoTracking,
        object id,
        Expression<Func<TEntity, TProjected>> projectExpression,
        CancellationToken cancellationToken = default) where TEntity : class;

    TProjected? GetById<TEntity, TProjected>(bool asNoTracking,
        object id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class;

    Task<TProjected?> GetByIdAsync<TEntity, TProjected>(bool asNoTracking,
        object id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        Expression<Func<TEntity, TProjected>> projectExpression,
        CancellationToken cancellationToken = default) where TEntity : class;

    T Add(T entity);

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    IEnumerable<T> AddRange(IEnumerable<T> entities);

    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    bool Any<TEntity>(Expression<Func<TEntity, bool>> anyExpression) where TEntity : class;

    Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> anyExpression, CancellationToken cancellationToken = default) where TEntity : class;

    T Update(T entity);

    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    IEnumerable<T> UpdateRange(IEnumerable<T> entities);

    Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> UpdateRangeAsync<TEntity>(
    IEnumerable<TEntity> entities,
    CancellationToken cancellationToken = default
    ) where TEntity : BaseEntity<TEntity>;

    void Enabled<TEntity>(TEntity entity, bool isEnabled) where TEntity : BaseEntity<TEntity>;

    Task EnabledAsync<TEntity>(TEntity entity,
        bool isEnabled,
        CancellationToken cancellationToken = default
        ) where TEntity : BaseEntity<TEntity>;

    void Enabled<TEntity>(object id, bool isEnabled) where TEntity : BaseEntity<TEntity>;

    Task EnabledAsync<TEntity>(object id,
        bool isEnabled,
        CancellationToken cancellationToken = default) where TEntity : BaseEntity<TEntity>;

    void SoftDelete<TEntity>(TEntity entity) where TEntity : BaseEntity<TEntity>;

    Task SoftDeleteAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TEntity>;

    void SoftDelete<TEntity>(object id) where TEntity : BaseEntity<TEntity>;

    Task SoftDeleteAsync<TEntity>(object id, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TEntity>;

    void Delete(T entity);

    Task DeleteAsync(T entity);

    int Count<TEntity>() where TEntity : class;

    Task<int> CountAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : class;

    int Count<TEntity>(Expression<Func<TEntity, bool>> whereExpression) where TEntity : class;

    Task<int> Count<TEntity>(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default) where TEntity : class;

    void SaveChanges();

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(IDbContextTransaction dbContextTransaction, CancellationToken cancellationToken = default);

}
public abstract class GenericRepository<T> : IGenericRepository<T>, IDisposable where T : class, new()
{
    protected readonly DbContext _context;
    private bool _isDispose = false;
    //public IUnitOfWork UnitOfWork { get { return (IUnitOfWork)_context; } }

    public GenericRepository(DbContext context)
    {
        _context = context;
    }
    public async Task<T?> GetByIdAsync(long id)
    {
        return await _context.Set<T>().FindAsync(id);
    }
    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _context
             .Set<T>()
             .ToListAsync();
    }
    public async Task<IReadOnlyList<T>> GetPagedReponseAsync(int pageNumber, int pageSize)
    {
        return await _context
            .Set<T>()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }
    public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class
    {
        return _context.Set<TEntity>().AsQueryable();
    }

    public IQueryable<TEntity> GetQueryable<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
    {
        return _context.Set<TEntity>().Where(filter);
    }

    public List<TEntity> GetMultiple<TEntity>(bool asNoTracking) where TEntity : class
    {
        return GetQueryable<TEntity>(asNoTracking).ToList();
    }

    public async Task<List<TEntity>> GetMultipleAsync<TEntity>(bool asNoTracking, CancellationToken cancellationToken = default) where TEntity : class
    {
        return await GetQueryable<TEntity>(asNoTracking).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public List<TProjected> GetMultiple<TEntity, TProjected>(bool asNoTracking, Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class
    {
        return GetQueryable<TEntity>(asNoTracking).Select(projectExpression).ToList();
    }

    public async Task<List<TProjected>> GetMultipleAsync<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, TProjected>> projectExpression,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        return await GetQueryable<TEntity>(asNoTracking).Select(projectExpression).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public List<TEntity> GetMultiple<TEntity>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression) where TEntity : class
    {
        return GetQueryable<TEntity>(asNoTracking).Where(whereExpression).ToList();
    }

    public async Task<List<TEntity>> GetMultipleAsync<TEntity>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        return await GetQueryable<TEntity>(asNoTracking).Where(whereExpression).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public List<TProjected> GetMultiple<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class
    {
        return GetQueryable<TEntity>(asNoTracking).Where(whereExpression).Select(projectExpression).ToList();
    }

    public async Task<List<TProjected>> GetMultipleAsync<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, TProjected>> projectExpression,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        return await GetQueryable<TEntity>(asNoTracking)
            .Where(whereExpression)
            .Select(projectExpression)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public List<TEntity> GetMultiple<TEntity>(bool asNoTracking,
        Func<IQueryable<TEntity>,
        IIncludableQueryable<TEntity, object>> includeExpression) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking);
        queryable = includeExpression(queryable);
        return queryable.ToList();
    }

    public async Task<List<TEntity>> GetMultipleAsync<TEntity>(bool asNoTracking,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity,
            object>> includeExpression,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking);
        queryable = includeExpression(queryable);
        return await queryable.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public List<TEntity> GetMultiple<TEntity>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(whereExpression);
        queryable = includeExpression(queryable);
        return queryable.ToList();
    }

    public async Task<List<TEntity>> GetMultipleAsync<TEntity>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(whereExpression);
        queryable = includeExpression(queryable);
        return await queryable.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public List<TProjected> GetMultiple<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity,
            object>> includeExpression, Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(whereExpression);
        queryable = includeExpression(queryable);
        return queryable.Select(projectExpression).ToList();
    }

    public async Task<List<TProjected>> GetMultipleAsync<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        Expression<Func<TEntity, TProjected>> projectExpression,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(whereExpression);
        queryable = includeExpression(queryable);
        return await queryable.Select(projectExpression).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public List<TEntity> GetMultiple<TEntity, TFilter>(bool asNoTracking, TFilter filter)
        where TEntity : class
        where TFilter : FilterBase
    {
        return GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter).ToList();
    }

    public async Task<List<TEntity>> GetMultipleAsync<TEntity, TFilter>(bool asNoTracking, TFilter filter, CancellationToken cancellationToken = default)
        where TEntity : class
        where TFilter : FilterBase
    {
        return await GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public List<TEntity> GetMultiple<TEntity, TFilter>(bool asNoTracking,
        TFilter filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression)
        where TEntity : class
        where TFilter : FilterBase
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
        queryable = includeExpression(queryable);
        return queryable.ToList();
    }

    public async Task<List<TEntity>> GetMultipleAsync<TEntity, TFilter>(bool asNoTracking,
        TFilter filter, Func<IQueryable<TEntity>,
            IIncludableQueryable<TEntity, object>> includeExpression, CancellationToken cancellationToken = default)
        where TEntity : class
        where TFilter : FilterBase
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
        queryable = includeExpression(queryable);
        return await queryable.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public List<TProjected> GetMultiple<TEntity, TFilter, TProjected>(bool asNoTracking,
        TFilter filter, Expression<Func<TEntity, TProjected>> projectExpression)
        where TEntity : class
        where TFilter : FilterBase
    {
        return GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter).Select(projectExpression).ToList();
    }

    public async Task<List<TProjected>> GetMultipleAsync<TEntity, TFilter, TProjected>(bool asNoTracking,
        TFilter filter, Expression<Func<TEntity, TProjected>> projectExpression,
        CancellationToken cancellationToken = default)
        where TEntity : class
        where TFilter : FilterBase
    {
        return await GetQueryable<TEntity>(asNoTracking)
            .ApplyFilter(filter)
            .Select(projectExpression)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public List<TProjected> GetMultiple<TEntity, TFilter, TProjected>(bool asNoTracking,
        TFilter filter, Expression<Func<TEntity, TProjected>> projectExpression,
        Func<IQueryable<TEntity>,
            IIncludableQueryable<TEntity, object>> includeExpression)
        where TEntity : class
        where TFilter : FilterBase
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
        queryable = includeExpression(queryable);
        return queryable.Select(projectExpression).ToList();
    }

    public async Task<List<TProjected>> GetMultipleAsync<TEntity, TFilter, TProjected>(bool asNoTracking,
        TFilter filter,
        Expression<Func<TEntity, TProjected>> projectExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default)
        where TEntity : class
        where TFilter : FilterBase
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
        queryable = includeExpression(queryable);
        return await queryable.Select(projectExpression).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public TEntity? GetSingle<TEntity>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(whereExpression);
        return queryable.FirstOrDefault();
    }

    public async Task<TEntity?> GetSingleAsync<TEntity>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(whereExpression);
        return await queryable.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    public TEntity? GetSingle<TEntity>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(whereExpression);
        queryable = includeExpression(queryable);
        return queryable.FirstOrDefault();
    }

    public async Task<TEntity?> GetSingleAsync<TEntity>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(whereExpression);
        queryable = includeExpression(queryable);
        return await queryable.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    public TProjected? GetSingle<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class
    {
        return GetQueryable<TEntity>(asNoTracking).Where(whereExpression).Select(projectExpression).FirstOrDefault();
    }

    public async Task<TProjected?> GetSingleAsync<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, TProjected>> projectExpression,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        return await GetQueryable<TEntity>(asNoTracking)
            .Where(whereExpression)
            .Select(projectExpression)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public TProjected? GetSingle<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, TProjected>> projectExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(whereExpression);
        queryable = includeExpression(queryable);
        return queryable.Select(projectExpression).FirstOrDefault();
    }

    public async Task<TProjected?> GetSingleAsync<TEntity, TProjected>(bool asNoTracking,
        Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, TProjected>> projectExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(whereExpression);
        queryable = includeExpression(queryable);
        return await queryable.Select(projectExpression)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public TEntity? GetSingle<TEntity, TFilter>(bool asNoTracking, TFilter filter)
        where TEntity : class
       where TFilter : FilterBase
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
        return queryable.FirstOrDefault();
    }

    public async Task<TEntity?> GetSingleAsync<TEntity, TFilter>(bool asNoTracking,
        TFilter filter,
        CancellationToken cancellationToken = default)
        where TEntity : class
        where TFilter : FilterBase
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
        return await queryable.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    public TEntity? GetSingle<TEntity, TFilter>(bool asNoTracking,
        TFilter filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression)
        where TEntity : class
        where TFilter : FilterBase
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
        queryable = includeExpression(queryable);
        return queryable.FirstOrDefault();
    }

    public async Task<TEntity?> GetSingleAsync<TEntity, TFilter>(bool asNoTracking,
        TFilter filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default)
        where TEntity : class
        where TFilter : FilterBase
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
        queryable = includeExpression(queryable);
        return await queryable.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    public TProjected? GetSingle<TEntity, TProjected, TFilter>(bool asNoTracking,
        TFilter filter,
        Expression<Func<TEntity, TProjected>> projectExpression)
        where TEntity : class
        where TFilter : FilterBase
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
        return queryable.Select(projectExpression).FirstOrDefault();
    }

    public async Task<TProjected?> GetSingleAsync<TEntity, TProjected, TFilter>(bool asNoTracking,
        TFilter filter,
        Expression<Func<TEntity, TProjected>> projectExpression,
        CancellationToken cancellationToken = default)
        where TEntity : class
        where TFilter : FilterBase
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
        return await queryable.Select(projectExpression).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    public TProjected? GetSingle<TEntity, TProjected, TFilter>(bool asNoTracking,
        TFilter filter,
        Expression<Func<TEntity, TProjected>> projectExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression)
        where TEntity : class
        where TFilter : FilterBase
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
        queryable = includeExpression(queryable);
        return queryable.Select(projectExpression).FirstOrDefault();
    }

    public async Task<TProjected?> GetSingleAsync<TEntity, TProjected, TFilter>(bool asNoTracking,
        TFilter filter,
        Expression<Func<TEntity, TProjected>> projectExpression,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression,
        CancellationToken cancellationToken = default)
        where TEntity : class
        where TFilter : FilterBase
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
        queryable = includeExpression(queryable);
        return await queryable.Select(projectExpression).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    public TEntity? GetById<TEntity>(object id) where TEntity : class
    {
        return _context.Set<TEntity>().FirstOrDefault(GenerateExpression<TEntity>(id));
    }

    public async Task<TEntity?> GetByIdAsync<TEntity>(bool asNoTracking,
        object id,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        return await GetQueryable<TEntity>(asNoTracking).FirstOrDefaultAsync(GenerateExpression<TEntity>(id), cancellationToken).ConfigureAwait(false);
    }

    public TEntity? GetById<TEntity>(bool asNoTracking, object id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(GenerateExpression<TEntity>(id));
        queryable = includeExpression(queryable);
        return queryable.FirstOrDefault();
    }

    public async Task<TEntity?> GetByIdAsync<TEntity>(bool asNoTracking, object id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, CancellationToken cancellationToken = default) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(GenerateExpression<TEntity>(id));
        queryable = includeExpression(queryable);
        return await queryable.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    public TProjected? GetById<TEntity, TProjected>(bool asNoTracking, object id, Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(GenerateExpression<TEntity>(id));
        return queryable.Select(projectExpression).FirstOrDefault();
    }

    public async Task<TProjected?> GetByIdAsync<TEntity, TProjected>(bool asNoTracking, object id, Expression<Func<TEntity, TProjected>> projectExpression, CancellationToken cancellationToken = default) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(GenerateExpression<TEntity>(id));
        return await queryable.Select(projectExpression).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    public TProjected? GetById<TEntity, TProjected>(bool asNoTracking, object id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(GenerateExpression<TEntity>(id));
        queryable = includeExpression(queryable);
        return queryable.Select(projectExpression).FirstOrDefault();
    }

    public async Task<TProjected?> GetByIdAsync<TEntity, TProjected>(bool asNoTracking, object id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, Expression<Func<TEntity, TProjected>> projectExpression, CancellationToken cancellationToken = default) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>(asNoTracking).Where(GenerateExpression<TEntity>(id));
        queryable = includeExpression(queryable);
        return await queryable
            .Select(projectExpression)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
    public T Add(T entity)
    {
        SetAuditCreate(entity);
        _context.Set<T>().Add(entity);
        return entity;
    }
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        SetAuditCreate(entity);
        await _context.Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return entity;
    }
    public IEnumerable<T> AddRange(IEnumerable<T> entities)
    {
        entities.ToList().ForEach(SetAuditCreate);
        _context.Set<T>().AddRange(entities);
        return entities;
    }
    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        entities.ToList().ForEach(SetAuditCreate);
        await _context.Set<T>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
        return entities;
    }
    public bool Any<TEntity>(Expression<Func<TEntity, bool>> anyExpression) where TEntity : class
    {
        return _context.Set<TEntity>().Any(anyExpression);
    }

    public async Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> anyExpression, CancellationToken cancellationToken = default) where TEntity : class
    {
        bool result = await _context.Set<TEntity>().AnyAsync(anyExpression, cancellationToken).ConfigureAwait(false);
        return result;
    }
    public T Update(T entity)
    {
        _context.Set<T>().Update(entity);
        return entity;
    }
    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default) 
    {
        _context.Entry(entity).State = EntityState.Modified;
        await Task.FromResult(entity);
    }
    public IEnumerable<T> UpdateRange(IEnumerable<T> entities)
    {
        _context.Set<T>().UpdateRange(entities);
        return entities;
    }

    public async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().UpdateRange(entities);
        return await Task.FromResult(entities);
    }

    public async Task<IEnumerable<TEntity>> UpdateRangeAsync<TEntity>(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
        ) where TEntity : BaseEntity<TEntity>
    {
        entities.ToList().ForEach(a => SetAuditUpdated(a));
        _context.Set<TEntity>().UpdateRange(entities);
        return await Task.FromResult(entities);
    }
    public void Enabled<TEntity>(TEntity entity, bool isEnabled) where TEntity : BaseEntity<TEntity>
    {
        entity.IsEnabled = isEnabled;
        Update(entity);
    }
    public void Enabled<TEntity>(object id, bool isEnabled) where TEntity : BaseEntity<TEntity>
    {
        var entity = _context.Set<TEntity>().FirstOrDefault(GenerateExpression<TEntity>(id));

        if (entity is not null)
        {
            entity.IsEnabled = isEnabled;
            Update(entity);
        }
    }

    public async Task EnabledAsync<TEntity>(TEntity entity,
        bool isEnabled,
        CancellationToken cancellationToken = default) where TEntity : BaseEntity<TEntity>
    {
        entity.IsEnabled = isEnabled;
        await UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task EnabledAsync<TEntity>(object id, bool isEnabled, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TEntity>
    {
        var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(GenerateExpression<TEntity>(id), cancellationToken).ConfigureAwait(false);

        if (entity is not null)
        {
            entity.IsEnabled = isEnabled;
            await UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
        }
    }
    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await Task.CompletedTask;
    }

    public void SoftDelete<TEntity>(TEntity entity) where TEntity : BaseEntity<TEntity>
    {
        SetIsDeleted(entity);
        Update(entity);
    }
    public void SoftDelete<TEntity>(object id) where TEntity : BaseEntity<TEntity>
    {
        var entity = _context.Set<TEntity>().FirstOrDefault(GenerateExpression<TEntity>(id));

        if (entity is not null)
        {
            SetIsDeleted(entity);
            Update(entity);
        }
    }

    public async Task SoftDeleteAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TEntity>
    {
        entity.IsDeleted = true;
        await UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task SoftDeleteAsync<TEntity>(object id, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TEntity>
    {
        var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(GenerateExpression<TEntity>(id), cancellationToken).ConfigureAwait(false);
        if (entity is not null)
        {
            SetIsDeleted(entity);
            await UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
        }
    }
    public int Count<TEntity>() where TEntity : class
    {
        return _context.Set<TEntity>().Count();
    }

    public int Count<TEntity>(Expression<Func<TEntity, bool>> whereExpression) where TEntity : class
    {
        return _context.Set<TEntity>().Where(whereExpression).Count();
    }

    public async Task<int> Count<TEntity>(Expression<Func<TEntity, bool>> whereExpression,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        int count = await _context.Set<TEntity>().Where(whereExpression).CountAsync(cancellationToken).ConfigureAwait(false);
        return count;
    }

    public async Task<int> CountAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : class
    {
        int count = await _context.Set<TEntity>().CountAsync(cancellationToken).ConfigureAwait(false);
        return count;
    }

    public void SetRowVersion<TEntity>(TEntity entity, byte[] version) where TEntity : BaseEntity<TEntity>
    {
        _context.Entry(entity).OriginalValues[nameof(entity.RowVersion)] = version;
    }

    public bool IsDbUpdateConcurrencyException(Exception ex)
    {
        return ex is DbUpdateConcurrencyException;
    }

    public void SaveChanges()
        => _context.SaveChanges();

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    protected virtual void Dispose(bool isDispose)
    {
        if (_isDispose) return;

        if (isDispose)
        {
            _context?.Dispose();
        }
        _isDispose = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        IDbContextTransaction dbContextTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return dbContextTransaction;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        => await _context.Database.RollbackTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(IDbContextTransaction dbContextTransaction, CancellationToken cancellationToken = default)
    {
        if (dbContextTransaction == null) ArgumentNullException.ThrowIfNull(dbContextTransaction);

        try
        {
            await SaveChangesAsync();
            await dbContextTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            RollbackTransaction(dbContextTransaction);
            throw;
        }
        finally
        {
            if (dbContextTransaction != null)
            {
                dbContextTransaction.Dispose();
            }
        }
    }

    private void RollbackTransaction(IDbContextTransaction dbContextTransaction, CancellationToken cancellationToken = default)
    {
        try
        {
            dbContextTransaction?.RollbackAsync(cancellationToken);
        }
        finally
        {
            if (dbContextTransaction != null)
            {
                dbContextTransaction.Dispose();
            }
        }
    }

    #region Queryable
    private IQueryable<TEntity> GetQueryable<TEntity>(bool asNoTracking) where TEntity : class
    {
        var queryable = GetQueryable<TEntity>();
        if (asNoTracking)
        {
            queryable = queryable.AsNoTracking();
        }
        return queryable;
    }
    private TEntity Update<TEntity>(TEntity entity) where TEntity : class
    {
        SetAuditUpdated(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    private Task<TEntity> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
    {
        SetAuditUpdated(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return Task.FromResult(entity);
    }
    private Expression<Func<TEntity, bool>> GenerateExpression<TEntity>(object id)
    {
        var type = _context.Model.FindEntityType(typeof(TEntity));
        string pk = type?.FindPrimaryKey()?.Properties.Select(s => s.Name).FirstOrDefault() ?? string.Empty;
        Type pkType = type?.FindPrimaryKey()?.Properties.Select(p => p.ClrType).FirstOrDefault();
        object value = Convert.ChangeType(id, pkType, CultureInfo.InvariantCulture);

        ParameterExpression pe = Expression.Parameter(typeof(TEntity), "entity");
        MemberExpression me = Expression.Property(pe, pk);
        ConstantExpression constant = Expression.Constant(value, pkType);
        BinaryExpression body = Expression.Equal(me, constant);
        Expression<Func<TEntity, bool>> expression = Expression.Lambda<Func<TEntity, bool>>(body, new[] { pe });

        return expression;
    }

    #endregion

    #region Set audit object
    private void SetAuditCreate(T obj)
    {
        if (obj is IAuditedEntity create)
        {
            create.CreatedDate = DateTime.Now;
        }
    }
    private void SetAuditUpdated<TEntity>(TEntity obj) where TEntity : class
    {
        if (obj is IAuditedEntity update)
        {

            update.UpdatedDate = DateTime.Now;
        }
    }

    private void SetIsDeleted<TEntity>(TEntity obj) where TEntity : class
    {

        if (obj is IDeleteEntity delete)
        {
            delete.IsDeleted = true;
        }
    }
    #endregion
}
