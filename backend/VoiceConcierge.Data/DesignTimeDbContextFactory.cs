using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VoiceConcierge.Data;

/// <summary>
/// Used by `dotnet ef` CLI at design time (migration generation, scaffolding).
/// Provides a placeholder connection string; no real DB connection is opened.
/// At runtime, DbContext is registered via DI with the actual connection string from config.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ConciergeDbContext>
{
    public ConciergeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConciergeDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=concierge;Username=concierge;Password=design_time_only",
            npgsql => npgsql.UseVector());

        return new ConciergeDbContext(optionsBuilder.Options);
    }
}
