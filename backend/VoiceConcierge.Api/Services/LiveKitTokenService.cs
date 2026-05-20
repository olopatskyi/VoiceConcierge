using Livekit.Server.Sdk.Dotnet;
using Microsoft.Extensions.Options;
using VoiceConcierge.Api.Common;
using VoiceConcierge.Api.Contracts.LiveKitToken;
using VoiceConcierge.Api.Models;
using VoiceConcierge.Api.Options;

namespace VoiceConcierge.Api.Services;

public interface ILiveKitTokenService
{
    Task<ServiceResponse<LiveKitTokenModel>> IssueAsync(LiveKitTokenRequest request);
}

public class LiveKitTokenService(IOptions<LiveKitOptions> options) : LogicalLayerElement, ILiveKitTokenService
{
    private readonly LiveKitOptions _options = options.Value;

    public Task<ServiceResponse<LiveKitTokenModel>> IssueAsync(LiveKitTokenRequest request)
    {
        var roomName = string.IsNullOrWhiteSpace(request.RoomName)
            ? $"concierge-{Guid.NewGuid():N}"
            : request.RoomName.Trim();

        // Identity must be unique per room — LiveKit kicks a duplicate-identity join.
        // Always generate fresh; supplied ParticipantName is treated as display name only.
        var identity = $"guest-{Guid.NewGuid():N}";

        var displayName = string.IsNullOrWhiteSpace(request.ParticipantName)
            ? "Guest"
            : request.ParticipantName.Trim();

        var token = new AccessToken(_options.ApiKey, _options.ApiSecret)
            .WithIdentity(identity)
            .WithName(displayName)
            .WithGrants(new VideoGrants
            {
                Room = roomName,
                RoomJoin = true,
                CanPublish = true,
                CanSubscribe = true,
                CanPublishData = true,
            })
            .WithTtl(TimeSpan.FromHours(1));

        var jwt = token.ToJwt();

        return Task.FromResult(Success(new LiveKitTokenModel
        {
            Token = jwt,
            Url = _options.Url,
            RoomName = roomName,
            ParticipantIdentity = identity,
            ParticipantDisplayName = displayName,
        }));
    }
}
