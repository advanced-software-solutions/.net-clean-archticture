namespace CleanBase
{
    public class EntityParent : IEntityParent
    {
        public int Id { get; set; }
        public byte[]? Rowversion { get; set; }
    }
}
