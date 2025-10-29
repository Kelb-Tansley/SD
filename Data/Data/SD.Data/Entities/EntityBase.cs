using System.ComponentModel.DataAnnotations;

namespace SD.Data.Entities;
public class EntityBase
{
    [Key]
    public int? Id { get; set; }
    public bool IsDeleted { get; set; }
    public required DateTime CreatedDate { get; set; }
    public required DateTime ModifiedDate { get; set; }
}

