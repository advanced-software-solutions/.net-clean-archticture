namespace CleanBase
{
    public class EntityRoot : IEntityRoot
    {
        public int Id { get; set; }
        public byte[]? Rowversion { get; set; }
    }
}
