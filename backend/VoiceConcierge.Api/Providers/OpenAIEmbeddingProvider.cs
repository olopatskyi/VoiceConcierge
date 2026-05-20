using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Pgvector;
using VoiceConcierge.Api.Common;
using VoiceConcierge.Api.Options;

namespace VoiceConcierge.Api.Providers;

/// <summary>
/// Calls https://api.openai.com/v1/embeddings via a typed HttpClient. HttpClient
/// is registered with AddStandardResilienceHandler so retries / circuit breaker /
/// timeouts are applied transparently.
/// </summary>
public class OpenAIEmbeddingProvider(HttpClient http, IOptions<OpenAIOptions> options)
    : LogicalLayerElement, IOpenAIEmbeddingProvider
{
    private const string Endpoint = "/v1/embeddings";

    private readonly OpenAIOptions _options = options.Value;

    public async Task<ServiceResponse<Vector>> EmbedAsync(string text, CancellationToken ct = default)
    {
        var batch = await EmbedBatchAsync(new[] { text }, ct);
        if (!batch.IsSuccess)
            return new ServiceResponse<Vector>
            {
                Status = batch.Status,
                ErrorMessage = batch.ErrorMessage,
            };

        return Success(batch.Value![0]);
    }

    public async Task<ServiceResponse<IReadOnlyList<Vector>>> EmbedBatchAsync(
        IReadOnlyList<string> texts,
        CancellationToken ct = default)
    {
        if (texts.Count == 0)
            return Success<IReadOnlyList<Vector>>(Array.Empty<Vector>());

        var payload = new EmbeddingsRequest(texts, _options.EmbeddingModel);

        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, Endpoint)
            {
                Content = JsonContent.Create(payload),
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

            using var resp = await http.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync(ct);
                return ExternalFailure<IReadOnlyList<Vector>>(
                    $"OpenAI embeddings returned {(int)resp.StatusCode} {resp.ReasonPhrase}: {Truncate(body, 200)}");
            }

            var parsed = await resp.Content.ReadFromJsonAsync<EmbeddingsResponse>(ct);
            if (parsed?.Data is null || parsed.Data.Count != texts.Count)
                return ExternalFailure<IReadOnlyList<Vector>>(
                    "OpenAI embeddings response shape mismatch.");

            // Order by index to be safe — the OpenAI contract guarantees input order, but be defensive.
            var ordered = parsed.Data
                .OrderBy(d => d.Index)
                .Select(d => new Vector(d.Embedding))
                .ToList();

            return Success<IReadOnlyList<Vector>>(ordered);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            return ExternalFailure<IReadOnlyList<Vector>>($"OpenAI embeddings unreachable: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            return ExternalFailure<IReadOnlyList<Vector>>($"OpenAI embeddings timed out: {ex.Message}");
        }
    }

    private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max] + "…";

    private sealed record EmbeddingsRequest(
        [property: JsonPropertyName("input")] IReadOnlyList<string> Input,
        [property: JsonPropertyName("model")] string Model);

    private sealed class EmbeddingsResponse
    {
        [JsonPropertyName("data")]
        public List<EmbeddingEntry>? Data { get; set; }
    }

    private sealed class EmbeddingEntry
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("embedding")]
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}
