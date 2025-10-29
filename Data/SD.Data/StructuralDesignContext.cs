

namespace SD.Data;
public class StructuralDesignContext : DbContext
{
    public string DbPath { get; }
    public StructuralDesignContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "StructuralDesign.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }
}
