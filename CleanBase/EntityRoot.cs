namespace CleanBase
{
    public class EntityRoot : IEntityRoot
    {
        public uint Id { get; set; }
        public byte[]? Rowversion { get; set; }
    }
}
