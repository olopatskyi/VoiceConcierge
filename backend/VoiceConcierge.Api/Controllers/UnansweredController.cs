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
    public async Task<IActionResult> GetManyAsync([FromQuery] GetManyUnansweredQuestionRequest request, CancellationToken ct) =>
        ActionResult(await service.GetManyAsync(request, ct));

    [HttpPost]
    [RequireAgentSecret]
    public async Task<IActionResult> RecordAsync([FromBody] RecordUnansweredQuestionRequest request, CancellationToken ct) =>
        ActionResult(await service.RecordAsync(request, ct));

    [HttpPost("{id:guid}/dismiss")]
    public async Task<IActionResult> DismissAsync(Guid id, CancellationToken ct) =>
        ActionResult(await service.DismissAsync(id, ct));

    [HttpPost("{id:guid}/convert")]
    public async Task<IActionResult> ConvertAsync(Guid id, [FromBody] ConvertToFaqRequest request, CancellationToken ct) =>
        ActionResult(await service.ConvertAsync(id, request, ct));
}
