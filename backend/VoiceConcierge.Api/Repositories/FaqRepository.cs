using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using VoiceConcierge.Api.Contracts.Faq;
using VoiceConcierge.Api.Models;
using VoiceConcierge.Data;
using VoiceConcierge.Data.Entities;

namespace VoiceConcierge.Api.Repositories;

public class FaqRepository(ConciergeDbContext db) : IFaqRepository
{
    public async Task<IReadOnlyList<FaqModel>> GetManyAsync(GetManyFaqRequest request, CancellationToken ct)
    {
        var entities = await db.FaqItems
            .AsNoTracking()
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);

        return entities.Select(MapToModel).ToList();
    }

    public async Task<FaqModel?> GetOneAsync(Guid id, CancellationToken ct)
    {
        var entity = await db.FaqItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return entity is null ? null : MapToModel(entity);
    }

    public async Task<FaqModel> CreateAsync(CreateFaqRequest request, Vector embedding, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var entity = new FaqItem
        {
            Id = Guid.NewGuid(),
            Question = request.Question,
            Answer = request.Answer,
            Category = request.Category,
            Embedding = embedding,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.FaqItems.Add(entity);
        await db.SaveChangesAsync(ct);

        return MapToModel(entity);
    }

    public async Task<FaqModel?> UpdateAsync(Guid id, UpdateFaqRequest request, Vector embedding, CancellationToken ct)
    {
        var entity = await db.FaqItems.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null)
            return null;

        entity.Question = request.Question;
        entity.Answer = request.Answer;
        entity.Category = request.Category;
        entity.Embedding = embedding;
        entity.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return MapToModel(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var deleted = await db.FaqItems
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync(ct);

        return deleted > 0;
    }

    public async Task<IReadOnlyList<FaqSearchResultModel>> SearchAsync(Vector queryEmbedding, int topK, CancellationToken ct)
    {
        // pgvector: <=> is cosine distance (0 = identical, 2 = opposite).
        // We convert to cosine similarity score (1 - distance) for caller convenience.
        var rows = await db.FaqItems
            .AsNoTracking()
            .Select(x => new
            {
                Entity = x,
                Distance = x.Embedding.CosineDistance(queryEmbedding),
            })
            .OrderBy(x => x.Distance)
            .Take(topK)
            .ToListAsync(ct);

        return rows
            .Select(r => new FaqSearchResultModel
            {
                Faq = MapToModel(r.Entity),
                Score = 1.0 - r.Distance,
            })
            .ToList();
    }

    private static FaqModel MapToModel(FaqItem entity) => new()
    {
        Id = entity.Id,
        Question = entity.Question,
        Answer = entity.Answer,
        Category = entity.Category,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
    };
}
