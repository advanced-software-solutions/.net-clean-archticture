using CleanBase;
using CleanBase.CleanAbstractions.CleanBusiness;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanOperation;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CleanBusiness
{
    public class RootService<T> : CleanAspects, IRootService<T> where T : class, IEntityRoot
    {
        public readonly IRepository<T> _repository;
        public RootService(IRepository<T> repository)
        {
            _repository = repository;
        }
        public void Delete(T entity)
        {
            _repository.Delete(entity);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public void Delete(List<T> entities)
        {
            _repository.Delete(entities);
        }

        public T? Get(int id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includedNavigations = null)
        {
            return _repository.Get(id, includedNavigations);
        }

        public T? Get(Expression<Func<T, bool>> query, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includedNavigations = null)
        {
            return _repository.Get(query, includedNavigations);
        }

        public async Task<T?> GetAsync(int id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includedNavigations = null)
        {
            return await _repository.GetAsync(id, includedNavigations);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> query, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includedNavigations = null)
        {
            return await _repository.GetAsync(query, includedNavigations);
        }

        public T Insert(T entity)
        {
            return _repository.Insert(entity);
        }

        public List<T> Insert(List<T> entities)
        {
            return _repository.Insert(entities);
        }

        public async Task<T?> InsertAsync(T entity)
        {
            return await _repository.InsertAsync(entity);
        }

        public async Task InsertAsync(List<T> entities)
        {
            await _repository.InsertAsync(entities);
        }

        public IQueryable<T?> Query()
        {
            return _repository.Query();
        }

        public EntityEntry<T> Update(T entity)
        {
            return _repository.Update(entity);
        }

        public void Update(List<T> entities)
        {
            _repository.Update(entities);
        }
    }
}
