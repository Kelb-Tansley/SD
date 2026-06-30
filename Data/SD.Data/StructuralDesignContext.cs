using Microsoft.Extensions.DependencyInjection;
using SD.Data.Entities;

namespace SD.Data;

public class StructuralDesignContext : DbContext
{
    private readonly string? DbPath;

    public StructuralDesignContext(IAppSettings appSettings)
    {
        DbPath = Path.Join(appSettings.StorageLocation, "StructuralDesign.db");
        Database.Migrate();
    }

    public StructuralDesignContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }

    public DbSet<FemFileEntity> FemFiles { get; set; } = null!;
    public DbSet<BeamKValueEntity> BeamKValues { get; set; } = null!;

    public DbSet<SectionDesignSetting> SectionDesignSettings { get; set; } = null!;
    public DbSet<ModelDesignSetting> ModelDesignSettings { get; set; } = null!;
}