using VoiceConcierge.Api.Common;

namespace VoiceConcierge.Api.Providers;

public interface IElevenLabsTtsProvider
{
    /// <summary>Synthesizes audio for the given text using the specified ElevenLabs voice id.
    /// Returns audio/mpeg bytes on success.</summary>
    Task<ServiceResponse<byte[]>> SynthesizeAsync(string voiceId, string text, CancellationToken ct = default);
}
