using FluentValidation.Results;

namespace VoiceConcierge.Api.Common;

public class ServiceResponse
{
    public ServiceResponseStatus Status { get; init; }

    public bool IsSuccess => Status == ServiceResponseStatus.Success;

    public ValidationResult? ValidationResult { get; init; }

    public string? ErrorMessage { get; init; }
}

public class ServiceResponse<T> : ServiceResponse
{
    public T? Value { get; init; }
}
