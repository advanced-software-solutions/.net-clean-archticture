using CleanBase;
using CleanOperation;

namespace CleanBusiness;

public interface IRootService<T> : IRepository<T> where T : class,IEntityRoot
{
}
