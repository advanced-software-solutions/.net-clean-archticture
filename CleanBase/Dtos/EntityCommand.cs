namespace CleanBase.Dtos
{
    public class EntityCommand<TEntity,TId>
    {
        public ActionType Action { get; set; }
        public TEntity? Entity { get; set; }
        public List<TEntity>? Entities { get; set; }
        public TId? Id { get; set; }
        public Dictionary<string,object>? Extra { get; set; }
    }

    public enum ActionType
    {
        Insert,
        InsertList,
        Update,
        Delete,
        GetById,
        GetPaginated
    }
}
