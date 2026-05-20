using FluentValidation.Results;

namespace VoiceConcierge.Api.Common;

/// <summary>
/// Base for all logical-layer services. Provides typed factory helpers so concrete services
/// return <see cref="ServiceResponse"/> uniformly instead of throwing or returning nulls.
/// </summary>
public abstract class LogicalLayerElement
{
    protected static ServiceResponse Success() =>
        new() { Status = ServiceResponseStatus.Success };

    protected static ServiceResponse<T> Success<T>(T value) =>
        new() { Status = ServiceResponseStatus.Success, Value = value };

    protected static ServiceResponse Failure(ValidationResult validationResult) =>
        new() { Status = ServiceResponseStatus.ValidationFailed, ValidationResult = validationResult };

    protected static ServiceResponse<T> Failure<T>(ValidationResult validationResult) =>
        new() { Status = ServiceResponseStatus.ValidationFailed, ValidationResult = validationResult };

    protected static ServiceResponse NotFound(string? message = null) =>
        new() { Status = ServiceResponseStatus.NotFound, ErrorMessage = message };

    protected static ServiceResponse<T> NotFound<T>(string? message = null) =>
        new() { Status = ServiceResponseStatus.NotFound, ErrorMessage = message };

    protected static ServiceResponse Conflict(string message) =>
        new() { Status = ServiceResponseStatus.Conflict, ErrorMessage = message };

    protected static ServiceResponse<T> Conflict<T>(string message) =>
        new() { Status = ServiceResponseStatus.Conflict, ErrorMessage = message };

    protected static ServiceResponse ExternalFailure(string message) =>
        new() { Status = ServiceResponseStatus.ExternalServiceFailed, ErrorMessage = message };

    protected static ServiceResponse<T> ExternalFailure<T>(string message) =>
        new() { Status = ServiceResponseStatus.ExternalServiceFailed, ErrorMessage = message };
}
