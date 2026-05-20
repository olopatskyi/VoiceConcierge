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
    public async Task<IActionResult> GetManyAsync([FromQuery] GetManyFaqRequest request, CancellationToken ct)
    {
        var serviceResponse = await service.GetManyAsync(request, ct);
        return ActionResult(serviceResponse);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOneAsync(Guid id, CancellationToken ct)
    {
        var serviceResponse = await service.GetOneAsync(id, ct);
        return ActionResult(serviceResponse);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateFaqRequest request, CancellationToken ct)
    {
        var serviceResponse = await service.CreateAsync(request, ct);
        return ActionResult(serviceResponse);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateFaqRequest request, CancellationToken ct)
    {
        var serviceResponse = await service.UpdateAsync(id, request, ct);
        return ActionResult(serviceResponse);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var serviceResponse = await service.DeleteAsync(id, ct);
        return ActionResult(serviceResponse);
    }

    [HttpPost("search")]
    [RequireAgentSecret]
    public async Task<IActionResult> SearchAsync([FromBody] SearchFaqRequest request, CancellationToken ct)
    {
        var serviceResponse = await service.SearchAsync(request, ct);
        return ActionResult(serviceResponse);
    }
}
