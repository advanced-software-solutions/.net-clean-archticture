using System.ComponentModel.DataAnnotations;

namespace CleanBase;

public interface IEntityRoot
{
    Guid Id { get; set; }
    [Timestamp]
    byte[] Rowversion { get; set; }
}
