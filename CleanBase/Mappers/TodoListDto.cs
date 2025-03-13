using CleanBase.Entities;
using Riok.Mapperly.Abstractions;

namespace CleanBase.Mappers
{
    public class TodoListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public IList<TodoItem>? TodoItems { get; set; }
        public byte[]? Rowversion { get; set; }
    }

    [Mapper]
    public static partial class TodoListDtoMapper
    {
        public  static partial TodoListDto MapTodoListToTodoListDto(TodoList todo);
    }
}
