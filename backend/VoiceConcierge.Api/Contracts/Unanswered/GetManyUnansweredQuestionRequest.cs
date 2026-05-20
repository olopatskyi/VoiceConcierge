using VoiceConcierge.Data.Enums;

namespace VoiceConcierge.Api.Contracts.Unanswered;

public class GetManyUnansweredQuestionRequest
{
    public UnansweredQuestionStatus? Status { get; set; }
}
