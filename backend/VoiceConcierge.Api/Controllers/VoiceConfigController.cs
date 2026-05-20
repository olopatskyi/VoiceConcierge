using Microsoft.AspNetCore.Mvc;
using VoiceConcierge.Api.Common;
using VoiceConcierge.Api.Contracts.VoiceConfig;
using VoiceConcierge.Api.Services;
using VoiceConcierge.Data.Enums;

namespace VoiceConcierge.Api.Controllers;

[ApiController]
public class VoiceConfigController(IVoiceConfigService service, IVoicePreviewService previewService) : BaseController
{
    [HttpGet("api/voice-config")]
    public async Task<IActionResult> GetAsync(CancellationToken ct)
    {
        var serviceResponse = await service.GetAsync(ct);
        return ActionResult(serviceResponse);
    }

    [HttpPut("api/voice-config")]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateVoiceConfigRequest request, CancellationToken ct)
    {
        var serviceResponse = await service.UpdateAsync(request, ct);
        return ActionResult(serviceResponse);
    }

    [HttpGet("api/voices")]
    public async Task<IActionResult> GetCatalogAsync(CancellationToken ct)
    {
        var serviceResponse = await service.GetCatalogAsync(ct);
        return ActionResult(serviceResponse);
    }

    [HttpPost("api/voices/{id}/preview")]
    public async Task<IActionResult> PreviewAsync(VoiceId id, CancellationToken ct)
    {
        var serviceResponse = await previewService.PreviewAsync(id, ct);
        if (!serviceResponse.IsSuccess)
            return ActionResult(serviceResponse);

        return File(serviceResponse.Value!, "audio/mpeg");
    }
}
