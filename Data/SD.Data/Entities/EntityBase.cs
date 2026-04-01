using System.ComponentModel.DataAnnotations;

namespace SD.Data.Entities;
public class EntityBase
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime ModifiedDate { get; set; } = DateTime.Now;
}