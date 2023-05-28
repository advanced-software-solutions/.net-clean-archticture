using CleanBase.CleanAbstractions.CleanBusiness;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanBase.Entities;

namespace CleanBusiness
{
    public class TodoListService : RootService<TodoList>, ITodoListService
    {
        public TodoListService(IRepository<TodoList> repository) : base(repository)
        {

        }
    }
}
