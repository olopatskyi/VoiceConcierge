using Pgvector;
using VoiceConcierge.Api.Common;

namespace VoiceConcierge.Api.Providers;

public interface IOpenAIEmbeddingProvider
{
    Task<ServiceResponse<Vector>> EmbedAsync(string text, CancellationToken ct = default);

    Task<ServiceResponse<IReadOnlyList<Vector>>> EmbedBatchAsync(
        IReadOnlyList<string> texts,
        CancellationToken ct = default);
}
