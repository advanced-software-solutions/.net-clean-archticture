using System.Text.Json.Serialization;

namespace CleanBase.Entities;

public partial class TodoItem : EntityRoot
{
    public string Title { get; set; }
    public bool Completed { get; set; }
    public Guid TodoListId { get; set; }
    public TodoList? TodoList { get; set; }
}

[JsonSerializable(typeof(List<TodoItem>))]
public partial class TodoItemContextList : JsonSerializerContext { }

[JsonSerializable(typeof(TodoItem))]
public partial class TodoItemContext : JsonSerializerContext { }