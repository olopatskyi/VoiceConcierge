using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using VoiceConcierge.Api.Contracts.Unanswered;
using VoiceConcierge.Api.Models;
using VoiceConcierge.Data;
using VoiceConcierge.Data.Entities;
using VoiceConcierge.Data.Enums;

namespace VoiceConcierge.Api.Repositories;

public class UnansweredRepository(ConciergeDbContext db) : IUnansweredRepository
{
    public async Task<IReadOnlyList<UnansweredQuestionModel>> GetManyAsync(GetManyUnansweredQuestionRequest request, CancellationToken ct)
    {
        var query = db.UnansweredQuestions.AsNoTracking();

        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);

        var entities = await query
            .OrderByDescending(x => x.Frequency)
            .ThenByDescending(x => x.LastAskedAt)
            .ToListAsync(ct);

        return entities.Select(MapToModel).ToList();
    }

    public async Task<UnansweredQuestionModel?> GetOneAsync(Guid id, CancellationToken ct)
    {
        var entity = await db.UnansweredQuestions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return entity is null ? null : MapToModel(entity);
    }

    public async Task<UnansweredQuestionModel?> FindSimilarPendingAsync(Vector embedding, double similarityThreshold, CancellationToken ct)
    {
        // cosine similarity > threshold  ⇔  cosine distance < 1 - threshold
        var maxDistance = 1.0 - similarityThreshold;

        var hit = await db.UnansweredQuestions
            .AsNoTracking()
            .Where(x => x.Status == UnansweredQuestionStatus.Pending)
            .Select(x => new
            {
                Entity = x,
                Distance = x.Embedding.CosineDistance(embedding),
            })
            .Where(x => x.Distance < maxDistance)
            .OrderBy(x => x.Distance)
            .FirstOrDefaultAsync(ct);

        return hit is null ? null : MapToModel(hit.Entity);
    }

    public async Task<UnansweredQuestionModel> CreateAsync(RecordUnansweredQuestionRequest request, Vector embedding, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var entity = new UnansweredQuestion
        {
            Id = Guid.NewGuid(),
            Question = request.Question,
            Embedding = embedding,
            Frequency = 1,
            Status = UnansweredQuestionStatus.Pending,
            FirstAskedAt = now,
            LastAskedAt = now,
        };

        db.UnansweredQuestions.Add(entity);
        await db.SaveChangesAsync(ct);

        return MapToModel(entity);
    }

    public async Task<UnansweredQuestionModel?> BumpFrequencyAsync(Guid id, CancellationToken ct)
    {
        // NOTE: tracked-entity bump has a small race window with concurrent callers
        // (lost-update can understate frequency by 1). Acceptable at PRD scale.
        var entity = await db.UnansweredQuestions.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null)
            return null;

        entity.Frequency += 1;
        entity.LastAskedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return MapToModel(entity);
    }

    public async Task<UnansweredQuestionModel?> DismissAsync(Guid id, CancellationToken ct)
    {
        var entity = await db.UnansweredQuestions.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null || entity.Status != UnansweredQuestionStatus.Pending)
            return null;

        entity.Status = UnansweredQuestionStatus.Dismissed;

        await db.SaveChangesAsync(ct);

        return MapToModel(entity);
    }

    public async Task<UnansweredQuestionModel?> MarkConvertedAsync(Guid id, Guid faqId, CancellationToken ct)
    {
        var entity = await db.UnansweredQuestions.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null || entity.Status != UnansweredQuestionStatus.Pending)
            return null;

        entity.Status = UnansweredQuestionStatus.Converted;
        entity.ConvertedToFaqId = faqId;

        await db.SaveChangesAsync(ct);

        return MapToModel(entity);
    }

    private static UnansweredQuestionModel MapToModel(UnansweredQuestion entity) => new()
    {
        Id = entity.Id,
        Question = entity.Question,
        Frequency = entity.Frequency,
        Status = entity.Status,
        FirstAskedAt = entity.FirstAskedAt,
        LastAskedAt = entity.LastAskedAt,
        ConvertedToFaqId = entity.ConvertedToFaqId,
    };
}
