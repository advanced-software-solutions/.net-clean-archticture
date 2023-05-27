namespace CleanBase.Entities
{
    public class TodoItem : EntityRoot
    {
        public string Title { get; set; }
        public bool Completed { get; set; }
    }
}
