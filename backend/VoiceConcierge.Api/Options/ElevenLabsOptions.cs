namespace VoiceConcierge.Api.Options;

public class ElevenLabsOptions
{
    public const string SectionName = "ElevenLabs";

    public string ApiKey { get; set; } = string.Empty;

    public string TtsModel { get; set; } = "eleven_turbo_v2_5";

    public string VoiceJames { get; set; } = string.Empty;
    public string VoiceSofia { get; set; } = string.Empty;
    public string VoiceMarcus { get; set; } = string.Empty;
    public string VoiceElena { get; set; } = string.Empty;
}
