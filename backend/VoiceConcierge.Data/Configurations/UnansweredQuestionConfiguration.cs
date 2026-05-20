using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoiceConcierge.Data.Entities;

namespace VoiceConcierge.Data.Configurations;

public class UnansweredQuestionConfiguration : IEntityTypeConfiguration<UnansweredQuestion>
{
    public void Configure(EntityTypeBuilder<UnansweredQuestion> builder)
    {
        builder.ToTable("unanswered_questions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Question)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Embedding)
            .HasColumnType($"vector({Embedding.Dimensions})")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Composite index for the admin queue listing:
        //   WHERE Status = ? ORDER BY Frequency DESC, LastAskedAt DESC
        builder.HasIndex(x => new { x.Status, x.Frequency, x.LastAskedAt })
            .IsDescending(false, true, true);

        builder.HasIndex(x => x.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");
    }
}
