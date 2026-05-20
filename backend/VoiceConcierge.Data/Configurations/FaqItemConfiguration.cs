using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoiceConcierge.Data.Entities;

namespace VoiceConcierge.Data.Configurations;

public class FaqItemConfiguration : IEntityTypeConfiguration<FaqItem>
{
    public void Configure(EntityTypeBuilder<FaqItem> builder)
    {
        builder.ToTable("faq_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Question)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Answer)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Embedding)
            .HasColumnType(Embedding.ColumnType)
            .IsRequired();

        builder.HasIndex(x => x.Category);

        builder.HasIndex(x => x.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");
    }
}
