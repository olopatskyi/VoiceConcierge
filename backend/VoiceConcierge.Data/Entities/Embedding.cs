namespace VoiceConcierge.Data.Entities;

public static class Embedding
{
    /// <summary>
    /// Vector dimensionality used across FAQ and unanswered-question embeddings.
    /// Currently matches OpenAI text-embedding-3-small output (1536).
    /// </summary>
    public const int Dimensions = 1536;

    /// <summary>
    /// Postgres column type for embedding columns.
    /// </summary>
    public static readonly string ColumnType = $"vector({Dimensions})";
}
