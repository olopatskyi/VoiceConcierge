using Pgvector;
using VoiceConcierge.Api.Contracts.Faq;
using VoiceConcierge.Api.Models;

namespace VoiceConcierge.Api.Repositories;

public interface IFaqRepository
{
    Task<IReadOnlyList<FaqModel>> GetManyAsync(GetManyFaqRequest request, CancellationToken ct);

    Task<FaqModel?> GetOneAsync(Guid id, CancellationToken ct);

    Task<FaqModel> CreateAsync(CreateFaqRequest request, Vector embedding, CancellationToken ct);

    Task<FaqModel?> UpdateAsync(Guid id, UpdateFaqRequest request, Vector embedding, CancellationToken ct);

    Task<bool> DeleteAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<FaqSearchResultModel>> SearchAsync(Vector queryEmbedding, int topK, CancellationToken ct);
}
