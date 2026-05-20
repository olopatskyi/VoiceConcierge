using Pgvector;

namespace VoiceConcierge.Data.Entities;

public class FaqItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Question { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Embedding of (Question + " " + Answer) using text-embedding-3-small (1536 dims).
    /// </summary>
    public Vector Embedding { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
