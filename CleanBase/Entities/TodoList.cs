namespace CleanBase.Entities
{
    public class TodoList : EntityRoot
    {
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public IList<TodoItem>? TodoItems { get; set; }
    }
}
