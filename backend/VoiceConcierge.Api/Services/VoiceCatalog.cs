using VoiceConcierge.Api.Models;
using VoiceConcierge.Data.Enums;

namespace VoiceConcierge.Api.Services;

/// <summary>
/// Hardcoded list of 4 voices per PRD VX-1.
/// Personality descriptions match the target voice character; actual ElevenLabs voice IDs
/// are mapped via <see cref="VoiceConcierge.Api.Options.ElevenLabsOptions"/>.
/// </summary>
public static class VoiceCatalog
{
    public static readonly IReadOnlyList<VoiceCatalogEntry> All = new VoiceCatalogEntry[]
    {
        new()
        {
            Id = VoiceId.James,
            Name = "James",
            Description = "Male, mature, warm British accent. Professional and refined.",
        },
        new()
        {
            Id = VoiceId.Sofia,
            Name = "Sofia",
            Description = "Female, friendly, subtle European accent. Welcoming and elegant.",
        },
        new()
        {
            Id = VoiceId.Marcus,
            Name = "Marcus",
            Description = "Male, American, confident and energetic. Modern and approachable.",
        },
        new()
        {
            Id = VoiceId.Elena,
            Name = "Elena",
            Description = "Female, American, calm and reassuring. Sophisticated and clear.",
        },
    };
}
