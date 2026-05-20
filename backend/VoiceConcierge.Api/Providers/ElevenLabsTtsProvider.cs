using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using VoiceConcierge.Api.Common;
using VoiceConcierge.Api.Options;

namespace VoiceConcierge.Api.Providers;

/// <summary>
/// Calls https://api.elevenlabs.io/v1/text-to-speech/{voiceId} via a typed HttpClient.
/// HttpClient is registered with AddStandardResilienceHandler for retries / timeouts.
/// </summary>
public class ElevenLabsTtsProvider(HttpClient http, IOptions<ElevenLabsOptions> options)
    : LogicalLayerElement, IElevenLabsTtsProvider
{
    private readonly ElevenLabsOptions _options = options.Value;

    public async Task<ServiceResponse<byte[]>> SynthesizeAsync(string voiceId, string text, CancellationToken ct = default)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, $"/v1/text-to-speech/{voiceId}")
            {
                Content = JsonContent.Create(new TtsRequest(text, _options.TtsModel)),
            };
            req.Headers.Add("xi-api-key", _options.ApiKey);
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("audio/mpeg"));

            using var resp = await http.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync(ct);
                return ExternalFailure<byte[]>(
                    $"ElevenLabs TTS returned {(int)resp.StatusCode} {resp.ReasonPhrase}: {Truncate(body, 200)}");
            }

            var audio = await resp.Content.ReadAsByteArrayAsync(ct);
            return Success(audio);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            return ExternalFailure<byte[]>($"ElevenLabs TTS unreachable: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            return ExternalFailure<byte[]>($"ElevenLabs TTS timed out: {ex.Message}");
        }
    }

    private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max] + "…";

    private sealed record TtsRequest(
        [property: JsonPropertyName("text")] string Text,
        [property: JsonPropertyName("model_id")] string ModelId);
}
