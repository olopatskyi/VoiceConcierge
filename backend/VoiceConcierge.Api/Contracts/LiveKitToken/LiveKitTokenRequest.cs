namespace VoiceConcierge.Api.Contracts.LiveKitToken;

public class LiveKitTokenRequest
{
    /// <summary>Optional. Caller-supplied participant name shown in the room. If empty, a random identity is generated.</summary>
    public string? ParticipantName { get; set; }

    /// <summary>Optional. Room to join. If empty, a fresh room name is generated so each playground session is isolated.</summary>
    public string? RoomName { get; set; }
}
