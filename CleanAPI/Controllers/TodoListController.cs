using CleanBase.CleanAbstractions.CleanBusiness;
using CleanBase.Entities;

namespace CleanAPI.Controllers
{
    public class TodoListController : BaseController<TodoList>
    {
        public TodoListController(ITodoListService todoListService):base(todoListService)
        {
            
        }
    }
}
