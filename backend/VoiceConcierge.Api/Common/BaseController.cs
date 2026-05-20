using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace VoiceConcierge.Api.Common;

public abstract class BaseController : ControllerBase
{
    protected IActionResult ActionResult(ServiceResponse response) =>
        BuildActionResult(response, hasValue: false, value: null);

    protected IActionResult ActionResult<T>(ServiceResponse<T> response) =>
        BuildActionResult(response, hasValue: true, value: response.Value);

    private static IActionResult BuildActionResult(ServiceResponse response, bool hasValue, object? value) =>
        response.Status switch
        {
            ServiceResponseStatus.Success when hasValue => new OkObjectResult(new { Data = value }),
            ServiceResponseStatus.Success => new NoContentResult(),

            ServiceResponseStatus.ValidationFailed =>
                new BadRequestObjectResult(BuildValidationProblem(response.ValidationResult!)),

            ServiceResponseStatus.NotFound =>
                new NotFoundObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not found",
                    Detail = response.ErrorMessage,
                }),

            ServiceResponseStatus.Conflict =>
                new ConflictObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflict",
                    Detail = response.ErrorMessage,
                }),

            ServiceResponseStatus.ExternalServiceFailed =>
                new ObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status502BadGateway,
                    Title = "Upstream service failure",
                    Detail = response.ErrorMessage,
                }) { StatusCode = StatusCodes.Status502BadGateway },

            _ => new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unhandled service response status",
            }) { StatusCode = StatusCodes.Status500InternalServerError },
        };

    private static ValidationProblemDetails BuildValidationProblem(ValidationResult result)
    {
        var errors = result.Errors
            .GroupBy(e => e.PropertyName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
        };
    }
}
