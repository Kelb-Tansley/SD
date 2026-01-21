using SD.Data.Entities;

namespace SD.Data;

public class StructuralDesignContext : DbContext
{
    private readonly string? _dbPath;

    public StructuralDesignContext(IAppSettings appSettings)
    {
        _dbPath = Path.Join(appSettings.StorageLocation, "StructuralDesign.db");
        Database.EnsureCreated();
    }

    public StructuralDesignContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={_dbPath}");
    }

    public DbSet<FemFileEntity> FemFiles { get; set; } = null!;
    public DbSet<BeamPropertySettings> BeamPropertySettings { get; set; } = null!;
    public DbSet<DesignSettings> DesignSettings { get; set; } = null!;
}

//dotnet ef migrations add 'First'