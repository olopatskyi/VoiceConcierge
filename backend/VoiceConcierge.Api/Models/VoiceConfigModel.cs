using VoiceConcierge.Data.Enums;

namespace VoiceConcierge.Api.Models;

public class VoiceConfigModel
{
    public VoiceId ActiveVoiceId { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public class VoiceCatalogEntry
{
    public required VoiceId Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }

    /// <summary>True when this entry matches the currently active voice. Set by the service when listing the catalog.</summary>
    public bool IsActive { get; init; }
}
