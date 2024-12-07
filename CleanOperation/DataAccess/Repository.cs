using Ardalis.GuardClauses;
using CleanBase;
using CleanBase.CleanAbstractions.CleanOperation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CleanOperation.DataAccess
{
    public class Repository<T> : RepoAspects, IRepository<T> where T : class, IEntityRoot
    {
        private readonly AppDataContext _dataContext;

        public Repository(AppDataContext dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
            Guard.Against.Null(_dataContext);
        }

        public void Delete(T entity)
        {
            Guard.Against.Null(entity, nameof(entity), "Please provide an entity");
            Aspect(() =>
            {
                _dataContext.Remove(entity);
            });
        }

        public void Delete(int id)
        {
            Guard.Against.NegativeOrZero(id);
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
                _dataContext.BulkDelete(entities);
            });
        }

        public T? Get(int id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? navigations = null)
        {
            Guard.Against.NegativeOrZero(id);
            return Aspect(() =>
            {
                var query = _dataContext.Set<T>().AsQueryable();
                if (navigations != null)
                {
                    query = navigations.Invoke(query);
                }
                return query.FirstOrDefault(y => y.Id == id);
            });
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

        public async Task<T?> GetAsync(int id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? navigations = null)
        {
            Guard.Against.NegativeOrZero(id);
            return await AspectAsync(async () =>
            {
                var query = _dataContext.Set<T>().AsQueryable();
                if (navigations != null)
                {
                    query = navigations.Invoke(query);
                }
                return await query.FirstOrDefaultAsync(y => y.Id == id);
            });
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
                _dataContext.BulkInsert(entities);
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
                await _dataContext.BulkInsertAsync(entities);
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
                _dataContext.BulkUpdate(entities);
            });
        }

        public DbContext GetAppDataContext() { return _dataContext; }
    }
}
