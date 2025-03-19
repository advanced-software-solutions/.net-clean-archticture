using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CleanBase.Entities;

[Table("[dbo].[TodoList]")]
public partial class TodoList : EntityRoot
{
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public IList<TodoItem>? TodoItems { get; set; }
}

