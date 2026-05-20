using VoiceConcierge.Data.Enums;

namespace VoiceConcierge.Data.Entities;

/// <summary>
/// Singleton row (Id = 1 always) holding the currently active concierge voice.
/// </summary>
public class VoiceConfig
{
    public const int SingletonId = 1;

    public int Id { get; set; } = SingletonId;

    public VoiceId ActiveVoiceId { get; set; } = VoiceId.Sofia;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
