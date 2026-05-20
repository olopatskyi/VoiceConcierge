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
    public async Task<IActionResult> GetAsync(CancellationToken ct) =>
        ActionResult(await service.GetAsync(ct));

    [HttpPut("api/voice-config")]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateVoiceConfigRequest request, CancellationToken ct) =>
        ActionResult(await service.UpdateAsync(request, ct));

    [HttpGet("api/voices")]
    public async Task<IActionResult> GetCatalogAsync(CancellationToken ct) =>
        ActionResult(await service.GetCatalogAsync(ct));

    [HttpPost("api/voices/{id}/preview")]
    public async Task<IActionResult> PreviewAsync(VoiceId id, CancellationToken ct)
    {
        var response = await previewService.PreviewAsync(id, ct);
        if (!response.IsSuccess)
            return ActionResult(response);

        return File(response.Value!, "audio/mpeg");
    }
}
