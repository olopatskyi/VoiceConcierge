namespace VoiceConcierge.Api.Options;

public class AgentAuthOptions
{
    public const string SectionName = "AgentAuth";

    /// <summary>
    /// Shared secret required in X-Agent-Secret header on agent → backend calls.
    /// </summary>
    public string SharedSecret { get; set; } = string.Empty;
}
