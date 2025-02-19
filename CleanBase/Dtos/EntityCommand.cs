namespace CleanBase.Dtos
{
    public class EntityCommand<T>
    {
        public ActionType Action { get; set; }
        public T Entity { get; set; }
    }

    public enum ActionType
    {

    }
}
