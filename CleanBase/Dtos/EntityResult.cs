namespace CleanBase.Dtos
{
    public class EntityResult<TEntity>
    {
        public TEntity? Data { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
        public Dictionary<string, object>? Details { get; set; }
    }
}
