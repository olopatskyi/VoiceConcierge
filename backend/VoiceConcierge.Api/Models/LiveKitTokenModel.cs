namespace VoiceConcierge.Api.Models;

public class LiveKitTokenModel
{
    public required string Token { get; init; }
    public required string Url { get; init; }
    public required string RoomName { get; init; }
    public required string ParticipantIdentity { get; init; }
    public required string ParticipantDisplayName { get; init; }
}
