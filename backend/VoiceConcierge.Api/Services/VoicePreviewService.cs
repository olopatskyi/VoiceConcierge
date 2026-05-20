using Microsoft.Extensions.Options;
using VoiceConcierge.Api.Common;
using VoiceConcierge.Api.Options;
using VoiceConcierge.Api.Providers;
using VoiceConcierge.Data.Enums;

namespace VoiceConcierge.Api.Services;

public interface IVoicePreviewService
{
    Task<ServiceResponse<byte[]>> PreviewAsync(VoiceId voice, CancellationToken ct);
}

public class VoicePreviewService(
    IElevenLabsTtsProvider tts,
    IOptions<ElevenLabsOptions> options) : LogicalLayerElement, IVoicePreviewService
{
    private const string SampleText =
        "Welcome to The Meridian Casino and Resort. How may I assist you this evening?";

    private readonly ElevenLabsOptions _options = options.Value;

    public async Task<ServiceResponse<byte[]>> PreviewAsync(VoiceId voice, CancellationToken ct)
    {
        var elevenVoiceId = MapVoiceId(voice);
        if (string.IsNullOrWhiteSpace(elevenVoiceId))
            return NotFound<byte[]>($"ElevenLabs voice id for {voice} is not configured");

        return await tts.SynthesizeAsync(elevenVoiceId, SampleText, ct);
    }

    private string MapVoiceId(VoiceId voice) => voice switch
    {
        VoiceId.James => _options.VoiceJames,
        VoiceId.Sofia => _options.VoiceSofia,
        VoiceId.Marcus => _options.VoiceMarcus,
        VoiceId.Elena => _options.VoiceElena,
        _ => string.Empty,
    };
}
