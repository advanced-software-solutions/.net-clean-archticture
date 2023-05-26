using System.ComponentModel.DataAnnotations;

namespace CleanBase
{
    public interface IEntityParent
    {
        int Id { get; set; }
        [Timestamp]
        byte[]? Rowversion { get; set; }
    }
}
