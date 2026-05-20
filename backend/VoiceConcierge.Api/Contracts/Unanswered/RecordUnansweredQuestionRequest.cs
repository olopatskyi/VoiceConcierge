using FluentValidation;

namespace VoiceConcierge.Api.Contracts.Unanswered;

public class RecordUnansweredQuestionRequest
{
    public string Question { get; set; } = string.Empty;
}

public class RecordUnansweredQuestionRequestValidator : AbstractValidator<RecordUnansweredQuestionRequest>
{
    public RecordUnansweredQuestionRequestValidator()
    {
        RuleFor(x => x.Question).NotEmpty().MaximumLength(1000);
    }
}
