using System.Text.Json.Serialization;
namespace CleanBase;

[QueryType]
public class EntityRoot : IEntityRoot
{
    public Guid Id { get; set; }
    public byte[]? Rowversion { get; set; }
}

