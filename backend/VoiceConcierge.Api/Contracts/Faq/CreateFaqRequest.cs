using FluentValidation;

namespace VoiceConcierge.Api.Contracts.Faq;

public class CreateFaqRequest
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

public class CreateFaqRequestValidator : AbstractValidator<CreateFaqRequest>
{
    public CreateFaqRequestValidator()
    {
        RuleFor(x => x.Question).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Answer).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
    }
}
