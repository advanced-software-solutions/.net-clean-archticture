using System.ComponentModel.DataAnnotations;

namespace CleanBase
{
    public interface IEntityRoot
    {
        uint Id { get; set; }
        [Timestamp]
        byte[]? Rowversion { get; set; }
    }
}
