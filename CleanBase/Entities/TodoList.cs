using System.Text.Json.Serialization;

namespace CleanBase.Entities;

public partial class TodoList : EntityRoot
{
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public IList<TodoItem>? TodoItems { get; set; }
}

