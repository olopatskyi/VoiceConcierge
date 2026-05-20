using FluentValidation;

namespace VoiceConcierge.Api.Contracts.Unanswered;

public class ConvertToFaqRequest
{
    public string Answer { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

public class ConvertToFaqRequestValidator : AbstractValidator<ConvertToFaqRequest>
{
    public ConvertToFaqRequestValidator()
    {
        RuleFor(x => x.Answer).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
    }
}
