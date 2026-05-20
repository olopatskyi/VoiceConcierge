using FluentValidation;
using VoiceConcierge.Api.Common;
using VoiceConcierge.Api.Contracts.Faq;
using VoiceConcierge.Api.Models;
using VoiceConcierge.Api.Providers;
using VoiceConcierge.Api.Repositories;

namespace VoiceConcierge.Api.Services;

public interface IFaqService
{
    Task<ServiceResponse<IReadOnlyList<FaqModel>>> GetManyAsync(GetManyFaqRequest request, CancellationToken ct);

    Task<ServiceResponse<FaqModel>> GetOneAsync(Guid id, CancellationToken ct);

    Task<ServiceResponse<FaqModel>> CreateAsync(CreateFaqRequest request, CancellationToken ct);

    Task<ServiceResponse<FaqModel>> UpdateAsync(Guid id, UpdateFaqRequest request, CancellationToken ct);

    Task<ServiceResponse> DeleteAsync(Guid id, CancellationToken ct);

    Task<ServiceResponse<IReadOnlyList<FaqSearchResultModel>>> SearchAsync(SearchFaqRequest request, CancellationToken ct);
}

public class FaqService(
    IFaqRepository repo,
    IOpenAIEmbeddingProvider embedding,
    IValidator<CreateFaqRequest> createValidator,
    IValidator<UpdateFaqRequest> updateValidator,
    IValidator<SearchFaqRequest> searchValidator) : LogicalLayerElement, IFaqService
{
    public async Task<ServiceResponse<IReadOnlyList<FaqModel>>> GetManyAsync(GetManyFaqRequest request, CancellationToken ct)
    {
        var models = await repo.GetManyAsync(request, ct);
        return Success(models);
    }

    public async Task<ServiceResponse<FaqModel>> GetOneAsync(Guid id, CancellationToken ct)
    {
        var model = await repo.GetOneAsync(id, ct);
        return model is null
            ? NotFound<FaqModel>($"FAQ {id} not found")
            : Success(model);
    }

    public async Task<ServiceResponse<FaqModel>> CreateAsync(CreateFaqRequest request, CancellationToken ct)
    {
        var validation = await createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Failure<FaqModel>(validation);

        var embeddingResponse = await embedding.EmbedAsync($"{request.Question} {request.Answer}", ct);
        if (!embeddingResponse.IsSuccess)
            return PropagateFailure<FaqModel>(embeddingResponse);

        var model = await repo.CreateAsync(request, embeddingResponse.Value!, ct);
        return Success(model);
    }

    public async Task<ServiceResponse<FaqModel>> UpdateAsync(Guid id, UpdateFaqRequest request, CancellationToken ct)
    {
        var validation = await updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Failure<FaqModel>(validation);

        var embeddingResponse = await embedding.EmbedAsync($"{request.Question} {request.Answer}", ct);
        if (!embeddingResponse.IsSuccess)
            return PropagateFailure<FaqModel>(embeddingResponse);

        var model = await repo.UpdateAsync(id, request, embeddingResponse.Value!, ct);
        return model is null
            ? NotFound<FaqModel>($"FAQ {id} not found")
            : Success(model);
    }

    public async Task<ServiceResponse> DeleteAsync(Guid id, CancellationToken ct)
    {
        var deleted = await repo.DeleteAsync(id, ct);
        return deleted ? Success() : NotFound($"FAQ {id} not found");
    }

    public async Task<ServiceResponse<IReadOnlyList<FaqSearchResultModel>>> SearchAsync(SearchFaqRequest request, CancellationToken ct)
    {
        var validation = await searchValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Failure<IReadOnlyList<FaqSearchResultModel>>(validation);

        var embeddingResponse = await embedding.EmbedAsync(request.Query, ct);
        if (!embeddingResponse.IsSuccess)
            return PropagateFailure<IReadOnlyList<FaqSearchResultModel>>(embeddingResponse);

        var results = await repo.SearchAsync(embeddingResponse.Value!, request.TopK, ct);
        return Success(results);
    }

    private static ServiceResponse<T> PropagateFailure<T>(ServiceResponse source) => new()
    {
        Status = source.Status,
        ValidationResult = source.ValidationResult,
        ErrorMessage = source.ErrorMessage,
    };
}
