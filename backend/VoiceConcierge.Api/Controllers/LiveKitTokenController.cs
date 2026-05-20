using Microsoft.AspNetCore.Mvc;
using VoiceConcierge.Api.Common;
using VoiceConcierge.Api.Contracts.LiveKitToken;
using VoiceConcierge.Api.Services;

namespace VoiceConcierge.Api.Controllers;

[ApiController]
[Route("api/livekit-token")]
public class LiveKitTokenController(ILiveKitTokenService service) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> IssueAsync([FromBody] LiveKitTokenRequest request) =>
        ActionResult(await service.IssueAsync(request));
}
