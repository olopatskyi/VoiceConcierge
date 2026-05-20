using FluentValidation;
using VoiceConcierge.Data.Enums;

namespace VoiceConcierge.Api.Contracts.VoiceConfig;

public class UpdateVoiceConfigRequest
{
    public VoiceId ActiveVoiceId { get; set; }
}

public class UpdateVoiceConfigRequestValidator : AbstractValidator<UpdateVoiceConfigRequest>
{
    public UpdateVoiceConfigRequestValidator()
    {
        RuleFor(x => x.ActiveVoiceId).IsInEnum();
    }
}
