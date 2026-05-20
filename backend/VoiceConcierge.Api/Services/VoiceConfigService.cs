using FluentValidation;
using VoiceConcierge.Api.Common;
using VoiceConcierge.Api.Contracts.VoiceConfig;
using VoiceConcierge.Api.Models;
using VoiceConcierge.Api.Repositories;

namespace VoiceConcierge.Api.Services;

public interface IVoiceConfigService
{
    Task<ServiceResponse<VoiceConfigModel>> GetAsync(CancellationToken ct);

    Task<ServiceResponse<VoiceConfigModel>> UpdateAsync(UpdateVoiceConfigRequest request, CancellationToken ct);

    Task<ServiceResponse<IReadOnlyList<VoiceCatalogEntry>>> GetCatalogAsync(CancellationToken ct);
}

public class VoiceConfigService(
    IVoiceConfigRepository repo,
    IValidator<UpdateVoiceConfigRequest> updateValidator) : LogicalLayerElement, IVoiceConfigService
{
    private const string NotInitializedMessage = "Voice configuration has not been initialized";

    public async Task<ServiceResponse<VoiceConfigModel>> GetAsync(CancellationToken ct)
    {
        var model = await repo.GetAsync(ct);
        return model is null
            ? NotFound<VoiceConfigModel>(NotInitializedMessage)
            : Success(model);
    }

    public async Task<ServiceResponse<VoiceConfigModel>> UpdateAsync(UpdateVoiceConfigRequest request, CancellationToken ct)
    {
        var validation = await updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Failure<VoiceConfigModel>(validation);

        var model = await repo.UpdateAsync(request.ActiveVoiceId, ct);
        return model is null
            ? NotFound<VoiceConfigModel>(NotInitializedMessage)
            : Success(model);
    }

    public async Task<ServiceResponse<IReadOnlyList<VoiceCatalogEntry>>> GetCatalogAsync(CancellationToken ct)
    {
        var config = await repo.GetAsync(ct);
        if (config is null)
            return NotFound<IReadOnlyList<VoiceCatalogEntry>>(NotInitializedMessage);

        var activeId = config.ActiveVoiceId;

        var entries = VoiceCatalog.All
            .Select(v => new VoiceCatalogEntry
            {
                Id = v.Id,
                Name = v.Name,
                Description = v.Description,
                IsActive = v.Id == activeId,
            })
            .ToList();

        return Success<IReadOnlyList<VoiceCatalogEntry>>(entries);
    }
}
