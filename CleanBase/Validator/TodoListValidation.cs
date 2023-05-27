using CleanBase.Entities;
using FluentValidation;

namespace CleanBase.Validator
{
    public class TodoListValidation : AbstractValidator<TodoList>
    {
        public TodoListValidation()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.DueDate).GreaterThanOrEqualTo(DateTime.Now);
        }
    }
}
