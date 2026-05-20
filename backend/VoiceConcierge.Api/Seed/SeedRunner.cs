using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoiceConcierge.Api.Providers;
using VoiceConcierge.Data;
using VoiceConcierge.Data.Entities;
using VoiceConcierge.Data.Enums;

namespace VoiceConcierge.Api.Seed;

/// <summary>
/// Idempotent startup seeding. Runs after migrations.
/// - Ensures the singleton voice_config row exists.
/// - Inserts FAQ items from <see cref="MeridianSeedData"/> if the table is empty (batched embedding).
/// FAQ seed failures (OpenAI unavailable, rate-limited) are logged but do not crash startup —
/// the app remains usable for admin work, and seeding retries on the next restart.
/// </summary>
public class SeedRunner(
    ConciergeDbContext db,
    IOpenAIEmbeddingProvider embedding,
    ILogger<SeedRunner> logger)
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        await EnsureVoiceConfigAsync(ct);
        await EnsureFaqsAsync(ct);
    }

    private async Task EnsureVoiceConfigAsync(CancellationToken ct)
    {
        if (await db.VoiceConfig.AnyAsync(ct))
            return;

        db.VoiceConfig.Add(new VoiceConfig
        {
            Id = VoiceConfig.SingletonId,
            ActiveVoiceId = VoiceId.Sofia,
            UpdatedAt = DateTime.UtcNow,
        });

        await db.SaveChangesAsync(ct);
    }

    private async Task EnsureFaqsAsync(CancellationToken ct)
    {
        if (await db.FaqItems.AnyAsync(ct))
            return;

        var seeds = MeridianSeedData.Faqs;
        if (seeds.Count == 0)
            return;

        var texts = seeds.Select(f => $"{f.Question} {f.Answer}").ToList();
        var embeddingResponse = await embedding.EmbedBatchAsync(texts, ct);

        if (!embeddingResponse.IsSuccess)
        {
            logger.LogError(
                "FAQ seed failed — embedding provider returned {Status}: {Message}. " +
                "App will start without seed; retry on next restart.",
                embeddingResponse.Status,
                embeddingResponse.ErrorMessage);
            return;
        }

        var embeddings = embeddingResponse.Value!;
        var now = DateTime.UtcNow;
        var entities = seeds
            .Zip(embeddings, (seed, emb) => new FaqItem
            {
                Id = Guid.NewGuid(),
                Question = seed.Question,
                Answer = seed.Answer,
                Category = seed.Category,
                Embedding = emb,
                CreatedAt = now,
                UpdatedAt = now,
            })
            .ToList();

        db.FaqItems.AddRange(entities);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Seeded {Count} FAQ items.", entities.Count);
    }
}
