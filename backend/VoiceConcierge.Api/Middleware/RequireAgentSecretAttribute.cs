namespace VoiceConcierge.Api.Middleware;

/// <summary>
/// Marker for endpoints callable only by the LiveKit voice agent.
/// <see cref="AgentSecretMiddleware"/> rejects requests without a valid X-Agent-Secret header.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class RequireAgentSecretAttribute : Attribute
{
}
