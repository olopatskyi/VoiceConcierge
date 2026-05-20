namespace VoiceConcierge.Api.Options;

public class LiveKitOptions
{
    public const string SectionName = "LiveKit";

    public string Url { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string ApiSecret { get; set; } = string.Empty;
}
