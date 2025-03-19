using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CleanBase.Entities;

[Table("[dbo].[TodoItem]")]
public partial class TodoItem : EntityRoot
{
    public string Title { get; set; }
    public bool Completed { get; set; }
    public Guid TodoListId { get; set; }
    public TodoList? TodoList { get; set; }
}

