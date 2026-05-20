namespace VoiceConcierge.Api.Common;

public enum ServiceResponseStatus
{
    Success,
    ValidationFailed,
    NotFound,
    Conflict,

    /// <summary>An external dependency (LLM, STT, TTS, embedding API) failed.</summary>
    ExternalServiceFailed,
}
