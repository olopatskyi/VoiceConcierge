using Pgvector;
using VoiceConcierge.Data.Enums;

namespace VoiceConcierge.Data.Entities;

public class UnansweredQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// Embedding of Question — used for semantic dedup (cosine > 0.85 → bump frequency).
    /// </summary>
    public Vector Embedding { get; set; } = null!;

    public int Frequency { get; set; } = 1;

    public UnansweredQuestionStatus Status { get; set; } = UnansweredQuestionStatus.Pending;

    public DateTime FirstAskedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastAskedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Set when admin converts this question to a FAQ. References FaqItem.Id.
    /// </summary>
    public Guid? ConvertedToFaqId { get; set; }
}
