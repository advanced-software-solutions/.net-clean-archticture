namespace CleanBase
{
    public class EntityRoot : IEntityRoot
    {
        public Guid Id { get; set; }
        public byte[]? Rowversion { get; set; }
    }
}
