﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CleanBase.CleanAbstractions.CleanOperation
{
    public interface IRepository<T> where T : class, IEntityRoot
    {
        T? Get(int id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includedNavigations = null);
        T? Get(Expression<Func<T, bool>> query, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includedNavigations = null);
        T Insert(T entity);
        EntityEntry<T> Update(T entity);
        void Delete(T entity);
        void Delete(int id);
        Task<T?> GetAsync(int id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includedNavigations = null);
        Task<T?> GetAsync(Expression<Func<T, bool>> query, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includedNavigations = null);
        Task<T?> InsertAsync(T entity);
        List<T> Insert(List<T> entities);
        Task InsertAsync(List<T> entity);
        void Update(List<T> entities);
        void Delete(List<T> entities);
        IQueryable<T?> Query();
    }
}
