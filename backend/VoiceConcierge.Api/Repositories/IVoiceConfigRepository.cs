using VoiceConcierge.Api.Models;
using VoiceConcierge.Data.Enums;

namespace VoiceConcierge.Api.Repositories;

public interface IVoiceConfigRepository
{
    /// <summary>Returns the singleton voice config, or null if it hasn't been seeded yet.</summary>
    Task<VoiceConfigModel?> GetAsync(CancellationToken ct);

    /// <summary>Updates the active voice. Returns null if the singleton row is missing.</summary>
    Task<VoiceConfigModel?> UpdateAsync(VoiceId newActiveVoiceId, CancellationToken ct);
}
