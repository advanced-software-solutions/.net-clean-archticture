using CleanBase.CleanAbstractions.CleanOperation;

namespace CleanBase.CleanAbstractions.CleanBusiness
{
    public interface IRootService<T> : IRepository<T> where T : class,IEntityRoot
    {
    }
}
