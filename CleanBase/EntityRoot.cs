using System.Text.Json.Serialization;
namespace CleanBase;


public class EntityRoot : IEntityRoot
{
    public Guid Id { get; set; }
    public byte[]? Rowversion { get; set; }
}

[JsonSerializable(typeof(List<EntityRoot>))]
public partial class EntityRootContextList : JsonSerializerContext { }

[JsonSerializable(typeof(EntityRoot))]
public partial class EntityRootContext : JsonSerializerContext { }