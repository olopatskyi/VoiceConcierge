using Microsoft.AspNetCore.Mvc;
using VoiceConcierge.Api.Common;
using VoiceConcierge.Api.Contracts.Faq;
using VoiceConcierge.Api.Middleware;
using VoiceConcierge.Api.Services;

namespace VoiceConcierge.Api.Controllers;

[ApiController]
[Route("api/faq")]
public class FaqController(IFaqService service) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetManyAsync([FromQuery] GetManyFaqRequest request, CancellationToken ct) =>
        ActionResult(await service.GetManyAsync(request, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOneAsync(Guid id, CancellationToken ct) =>
        ActionResult(await service.GetOneAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateFaqRequest request, CancellationToken ct) =>
        ActionResult(await service.CreateAsync(request, ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateFaqRequest request, CancellationToken ct) =>
        ActionResult(await service.UpdateAsync(id, request, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct) =>
        ActionResult(await service.DeleteAsync(id, ct));

    [HttpPost("search")]
    [RequireAgentSecret]
    public async Task<IActionResult> SearchAsync([FromBody] SearchFaqRequest request, CancellationToken ct) =>
        ActionResult(await service.SearchAsync(request, ct));
}
