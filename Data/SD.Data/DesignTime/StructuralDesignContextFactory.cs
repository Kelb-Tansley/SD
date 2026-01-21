using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SD.Data.DesignTime;

public class StructuralDesignContextFactory : IDesignTimeDbContextFactory<StructuralDesignContext>
{
    public StructuralDesignContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StructuralDesignContext>();
        optionsBuilder.UseSqlite($"Data Source=DesignTime.db");
        return new StructuralDesignContext(optionsBuilder.Options);
    }
}
