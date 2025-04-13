using Ardalis.GuardClauses;
using CleanBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;

using System.Linq.Expressions;

namespace CleanOperation.DataAccess;

public class Repository<T> : RepoAspects, IRepository<T> where T : class, IEntityRoot
{
    private readonly AppDataContext _dataContext;
    private readonly IConfiguration _configuration;
    public Repository(AppDataContext dataContext, IConfiguration configuration) : base(dataContext)
    {
        _dataContext = dataContext;
        Guard.Against.Null(_dataContext);
        _configuration = configuration;
    }

    public void Delete(T entity)
    {
        Guard.Against.Null(entity, nameof(entity), "Please provide an entity");
        Aspect(() =>
        {
            _dataContext.Remove(entity);
        });
    }

    public void Delete(Guid id)
    {
        Guard.Against.NullOrEmpty(id);
        Aspect(() =>
        {
            var item = Get(id);
            _dataContext.Remove(item);
        });
    }

    public void Delete(List<T> entities)
    {
        Guard.Against.Null(entities);
        Aspect(() =>
        {
            _dataContext.Set<T>().RemoveRange(entities);
        });
    }

    public T? Get(Guid id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? navigations = null)
    {
        Guard.Against.NullOrEmpty(id);
        var query = _dataContext.Set<T>().AsQueryable();
        if (navigations != null)
        {
            query = navigations.Invoke(query);
        }
        return query.FirstOrDefault(y => y.Id == id);
    }

    public T? Get(Expression<Func<T, bool>> query, Func<IQueryable<T>, IIncludableQueryable<T, object>>? navigations = null)
    {
        Guard.Against.Null(query);
        return Aspect(() =>
        {
            var querable = _dataContext.Set<T>().AsQueryable();
            if (navigations != null)
            {
                querable = navigations.Invoke(querable);
            }
            return querable.FirstOrDefault(query);
        });
    }

    public async Task<T?> GetAsync(Guid id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? navigations = null)
    {
        Guard.Against.NullOrEmpty(id);
        var query = _dataContext.Set<T>().AsQueryable();
        if (navigations != null)
        {
            query = navigations.Invoke(query);
        }
        return await query.FirstOrDefaultAsync(y => y.Id == id);
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> query, Func<IQueryable<T>, IIncludableQueryable<T, object>>? navigations = null)
    {
        Guard.Against.Null(query);
        return await AspectAsync(async () =>
        {
            var querable = _dataContext.Set<T>().AsQueryable();
            if (navigations != null)
            {
                querable = navigations.Invoke(querable);
            }
            return await querable.FirstOrDefaultAsync(query);
        });
    }

    public T Insert(T entity)
    {
        Guard.Against.Null(entity);
        return Aspect(() =>
        {
            _dataContext.Add(entity);
            return entity;
        });
    }

    public List<T> Insert(List<T> entities)
    {
        Guard.Against.NullOrEmpty(entities);
        return Aspect(() =>
        {
            _dataContext.Set<T>().AddRange(entities);
            return entities;
        });
    }

    public async Task<T?> InsertAsync(T entity)
    {
        Guard.Against.Null(entity);
        return await AspectAsync(async () =>
        {
            await _dataContext.AddAsync(entity);
            return entity;
        });
    }

    public async Task InsertAsync(List<T> entities)
    {
        Guard.Against.NullOrEmpty(entities);
        await AspectVoidAsync(async () =>
        {
            await _dataContext.Set<T>().AddRangeAsync(entities);
        });
    }

    public IQueryable<T?> Query()
    {
        return _dataContext.Set<T>().AsQueryable();
    }

    public EntityEntry<T> Update(T entity)
    {
        Guard.Against.Null(entity);
        return Aspect(() =>
        {
            var entry = _dataContext.Update(entity);
            return entry;
        });
    }

    public void Update(List<T> entities)
    {
        Guard.Against.NullOrEmpty(entities);
        Aspect(() =>
        {
            _dataContext.Set<T>().UpdateRange(entities);
        });
    }

    public DbContext GetAppDataContext() { return _dataContext; }
}
