using VoiceConcierge.Data.Enums;

namespace VoiceConcierge.Api.Models;

public class UnansweredQuestionModel
{
    public Guid Id { get; init; }
    public string Question { get; init; } = string.Empty;
    public int Frequency { get; init; }
    public UnansweredQuestionStatus Status { get; init; }
    public DateTime FirstAskedAt { get; init; }
    public DateTime LastAskedAt { get; init; }
    public Guid? ConvertedToFaqId { get; init; }
}
