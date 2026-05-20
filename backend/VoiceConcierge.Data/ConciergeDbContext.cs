using Microsoft.EntityFrameworkCore;
using VoiceConcierge.Data.Entities;

namespace VoiceConcierge.Data;

public class ConciergeDbContext(DbContextOptions<ConciergeDbContext> options) : DbContext(options)
{
    public DbSet<FaqItem> FaqItems => Set<FaqItem>();

    public DbSet<UnansweredQuestion> UnansweredQuestions => Set<UnansweredQuestion>();

    public DbSet<VoiceConfig> VoiceConfig => Set<VoiceConfig>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConciergeDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
