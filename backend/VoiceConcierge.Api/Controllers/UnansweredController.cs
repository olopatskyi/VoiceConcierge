using Microsoft.AspNetCore.Mvc;
using VoiceConcierge.Api.Common;
using VoiceConcierge.Api.Contracts.Unanswered;
using VoiceConcierge.Api.Middleware;
using VoiceConcierge.Api.Services;

namespace VoiceConcierge.Api.Controllers;

[ApiController]
[Route("api/unanswered")]
public class UnansweredController(IUnansweredService service) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetManyAsync([FromQuery] GetManyUnansweredQuestionRequest request, CancellationToken ct)
    {
        var serviceResponse = await service.GetManyAsync(request, ct);
        return ActionResult(serviceResponse);
    }

    [HttpPost]
    [RequireAgentSecret]
    public async Task<IActionResult> RecordAsync([FromBody] RecordUnansweredQuestionRequest request, CancellationToken ct)
    {
        var serviceResponse = await service.RecordAsync(request, ct);
        return ActionResult(serviceResponse);
    }

    [HttpPost("{id:guid}/dismiss")]
    public async Task<IActionResult> DismissAsync(Guid id, CancellationToken ct)
    {
        var serviceResponse = await service.DismissAsync(id, ct);
        return ActionResult(serviceResponse);
    }

    [HttpPost("{id:guid}/convert")]
    public async Task<IActionResult> ConvertAsync(Guid id, [FromBody] ConvertToFaqRequest request, CancellationToken ct)
    {
        var serviceResponse = await service.ConvertAsync(id, request, ct);
        return ActionResult(serviceResponse);
    }
}
