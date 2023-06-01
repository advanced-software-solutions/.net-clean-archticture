using System.ComponentModel.DataAnnotations;

namespace CleanBase
{
    public interface IEntityRoot
    {
        int Id { get; set; }
        [Timestamp]
        byte[] Rowversion { get; set; }
    }
}
