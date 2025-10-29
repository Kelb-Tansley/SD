using System.ComponentModel.DataAnnotations;

namespace SD.Data.Entities;
public class RecentFemFiles
{
    [Key]
    public int Id { get; set; }

    public required string FilePath { get; set; }
}
