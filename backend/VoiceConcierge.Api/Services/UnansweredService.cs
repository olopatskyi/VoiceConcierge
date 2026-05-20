using FluentValidation;
using VoiceConcierge.Api.Common;
using VoiceConcierge.Api.Contracts.Faq;
using VoiceConcierge.Api.Contracts.Unanswered;
using VoiceConcierge.Api.Models;
using VoiceConcierge.Api.Providers;
using VoiceConcierge.Api.Repositories;
using VoiceConcierge.Data.Enums;

namespace VoiceConcierge.Api.Services;

public interface IUnansweredService
{
    Task<ServiceResponse<IReadOnlyList<UnansweredQuestionModel>>> GetManyAsync(GetManyUnansweredQuestionRequest request, CancellationToken ct);

    Task<ServiceResponse<UnansweredQuestionModel>> RecordAsync(RecordUnansweredQuestionRequest request, CancellationToken ct);

    Task<ServiceResponse<UnansweredQuestionModel>> DismissAsync(Guid id, CancellationToken ct);

    Task<ServiceResponse<UnansweredQuestionModel>> ConvertAsync(Guid id, ConvertToFaqRequest request, CancellationToken ct);
}

public class UnansweredService(
    IUnansweredRepository repo,
    IOpenAIEmbeddingProvider embedding,
    IFaqService faqService,
    IValidator<RecordUnansweredQuestionRequest> recordValidator,
    IValidator<ConvertToFaqRequest> convertValidator) : LogicalLayerElement, IUnansweredService
{
    private const double DedupSimilarityThreshold = 0.85;

    public async Task<ServiceResponse<IReadOnlyList<UnansweredQuestionModel>>> GetManyAsync(GetManyUnansweredQuestionRequest request, CancellationToken ct)
    {
        var models = await repo.GetManyAsync(request, ct);
        return Success(models);
    }

    public async Task<ServiceResponse<UnansweredQuestionModel>> RecordAsync(RecordUnansweredQuestionRequest request, CancellationToken ct)
    {
        var validation = await recordValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Failure<UnansweredQuestionModel>(validation);

        var embeddingResponse = await embedding.EmbedAsync(request.Question, ct);
        if (!embeddingResponse.IsSuccess)
            return PropagateFailure<UnansweredQuestionModel>(embeddingResponse);

        var embeddingVector = embeddingResponse.Value!;

        var existing = await repo.FindSimilarPendingAsync(embeddingVector, DedupSimilarityThreshold, ct);

        UnansweredQuestionModel model;
        if (existing is null)
        {
            model = await repo.CreateAsync(request, embeddingVector, ct);
        }
        else
        {
            // Row may have been dismissed/converted between FindSimilar and Bump.
            // If so, fall back to inserting a fresh entry instead of crashing.
            var bumped = await repo.BumpFrequencyAsync(existing.Id, ct);
            model = bumped ?? await repo.CreateAsync(request, embeddingVector, ct);
        }

        return Success(model);
    }

    public async Task<ServiceResponse<UnansweredQuestionModel>> DismissAsync(Guid id, CancellationToken ct)
    {
        var existing = await repo.GetOneAsync(id, ct);
        if (existing is null)
            return NotFound<UnansweredQuestionModel>($"Unanswered question {id} not found");

        if (existing.Status != UnansweredQuestionStatus.Pending)
            return Conflict<UnansweredQuestionModel>($"Question is already {existing.Status}");

        var updated = await repo.DismissAsync(id, ct);
        return updated is null
            ? Conflict<UnansweredQuestionModel>("Question was modified concurrently")
            : Success(updated);
    }

    public async Task<ServiceResponse<UnansweredQuestionModel>> ConvertAsync(Guid id, ConvertToFaqRequest request, CancellationToken ct)
    {
        var validation = await convertValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Failure<UnansweredQuestionModel>(validation);

        var existing = await repo.GetOneAsync(id, ct);
        if (existing is null)
            return NotFound<UnansweredQuestionModel>($"Unanswered question {id} not found");

        if (existing.Status != UnansweredQuestionStatus.Pending)
            return Conflict<UnansweredQuestionModel>($"Question is already {existing.Status}");

        var createFaq = new CreateFaqRequest
        {
            Question = existing.Question,
            Answer = request.Answer,
            Category = request.Category,
        };

        var faqResponse = await faqService.CreateAsync(createFaq, ct);
        if (!faqResponse.IsSuccess)
            return PropagateFailure<UnansweredQuestionModel>(faqResponse);

        var updated = await repo.MarkConvertedAsync(id, faqResponse.Value!.Id, ct);
        return updated is null
            ? Conflict<UnansweredQuestionModel>("Question was modified concurrently")
            : Success(updated);
    }

    private static ServiceResponse<T> PropagateFailure<T>(ServiceResponse source) => new()
    {
        Status = source.Status,
        ValidationResult = source.ValidationResult,
        ErrorMessage = source.ErrorMessage,
    };
}
