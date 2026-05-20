namespace VoiceConcierge.Api.Models;

public class FaqModel
{
    public Guid Id { get; init; }
    public string Question { get; init; } = string.Empty;
    public string Answer { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public class FaqSearchResultModel
{
    public required FaqModel Faq { get; init; }
    public double Score { get; init; }
}
