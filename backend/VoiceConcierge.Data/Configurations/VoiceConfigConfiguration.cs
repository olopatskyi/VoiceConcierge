using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoiceConcierge.Data.Entities;
using VoiceConcierge.Data.Enums;

namespace VoiceConcierge.Data.Configurations;

public class VoiceConfigConfiguration : IEntityTypeConfiguration<VoiceConfig>
{
    public void Configure(EntityTypeBuilder<VoiceConfig> builder)
    {
        builder.ToTable("voice_config", t =>
            t.HasCheckConstraint("ck_voice_config_singleton", "\"Id\" = 1"));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ActiveVoiceId)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
    }
}
