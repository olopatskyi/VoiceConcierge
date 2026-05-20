using FluentValidation;

namespace VoiceConcierge.Api.Contracts.Faq;

public class SearchFaqRequest
{
    public string Query { get; set; } = string.Empty;
    public int TopK { get; set; } = 3;
}

public class SearchFaqRequestValidator : AbstractValidator<SearchFaqRequest>
{
    public SearchFaqRequestValidator()
    {
        RuleFor(x => x.Query).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.TopK).InclusiveBetween(1, 10);
    }
}
