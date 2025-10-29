using SD.Data.Entities;
using System.Reflection;

namespace SD.Data.Context;
public partial class RecentFemFilesContext : DbContext
{
    public DbSet<RecentFemFiles> RecentFemFiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // configure the database provider if one has not already been configured
        // Purpose if another provider is used for testing
        if (!optionsBuilder.IsConfigured)
        {

            optionsBuilder.UseSqlite("Data Source=" + LocalDBFileLocation() + "\\Published Items\\SDT_Reference.db");
        }
    }

    private static string? LocalDBFileLocation()
    {
        // location of exe file path
        return System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
    }
}
