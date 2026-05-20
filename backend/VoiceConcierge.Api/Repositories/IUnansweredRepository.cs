using Pgvector;
using VoiceConcierge.Api.Contracts.Unanswered;
using VoiceConcierge.Api.Models;

namespace VoiceConcierge.Api.Repositories;

public interface IUnansweredRepository
{
    Task<IReadOnlyList<UnansweredQuestionModel>> GetManyAsync(GetManyUnansweredQuestionRequest request, CancellationToken ct);

    Task<UnansweredQuestionModel?> GetOneAsync(Guid id, CancellationToken ct);

    /// <summary>Find a Pending question whose embedding is within (1 - similarityThreshold) cosine distance.</summary>
    Task<UnansweredQuestionModel?> FindSimilarPendingAsync(Vector embedding, double similarityThreshold, CancellationToken ct);

    Task<UnansweredQuestionModel> CreateAsync(RecordUnansweredQuestionRequest request, Vector embedding, CancellationToken ct);

    /// <summary>Atomic increment of Frequency + update LastAskedAt. Returns refreshed model.</summary>
    Task<UnansweredQuestionModel?> BumpFrequencyAsync(Guid id, CancellationToken ct);

    Task<UnansweredQuestionModel?> DismissAsync(Guid id, CancellationToken ct);

    Task<UnansweredQuestionModel?> MarkConvertedAsync(Guid id, Guid faqId, CancellationToken ct);
}
