using System.Text.Json.Serialization;

namespace CleanBase.Entities;

public partial class TodoList : EntityRoot
{
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public IList<TodoItem>? TodoItems { get; set; }
}

[JsonSerializable(typeof(List<TodoList>))]
public partial class TodoListContextList : JsonSerializerContext { }

[JsonSerializable(typeof(TodoList))]
public partial class TodoListContext : JsonSerializerContext { }