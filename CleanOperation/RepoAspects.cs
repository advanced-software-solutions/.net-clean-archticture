using CleanOperation.DataAccess;

namespace CleanOperation
{
    public class RepoAspects : CleanAspects
    {
        AppDataContext _dataContext;

        public RepoAspects(AppDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public override void Aspect(Action operation)
        {
            if (_dataContext.Database.CurrentTransaction != null)
            {
                base.Aspect(operation);
            }
            else
            {
                using (var txn = _dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        base.Aspect(operation);
                        _dataContext.SaveChanges();
                        txn.Commit();
                    }
                    catch (Exception)
                    {
                        txn.Rollback();
                        throw;
                    }
                }
            }
        }
        public override T Aspect<T>(Func<T> operation)
        {
            if (_dataContext.Database.CurrentTransaction != null)
            {
                return base.Aspect(operation);
            }
            else
            {
                using (var txn = _dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        var items = base.Aspect(operation);
                        _dataContext.SaveChanges();
                        txn.Commit();
                        return items;
                    }
                    catch (Exception)
                    {
                        txn.Rollback();
                        throw;
                    }
                }
            }
        }
        public override async Task<TResult> AspectAsync<TResult>(Func<Task<TResult>> operation)
        {
            if (_dataContext.Database.CurrentTransaction != null)
            {
                return await base.AspectAsync(operation);
            }
            else
            {
                using (var txn = _dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        var items = base.AspectAsync(operation);
                        _dataContext.SaveChanges();
                        await txn.CommitAsync();
                        return await items;
                    }
                    catch (Exception)
                    {
                        txn.Rollback();
                        throw;
                    }
                }
            }
        }
        public override async Task AspectVoidAsync(Func<Task> operation)
        {
            if (_dataContext.Database.CurrentTransaction != null)
            {
                await base.AspectVoidAsync(operation);
            }
            else
            {
                using (var txn = _dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        await base.AspectVoidAsync(operation);
                        _dataContext.SaveChanges();
                        await txn.CommitAsync();
                    }
                    catch (Exception)
                    {
                        txn.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
