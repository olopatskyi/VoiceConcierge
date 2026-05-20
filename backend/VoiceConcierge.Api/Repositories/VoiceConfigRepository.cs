using Microsoft.EntityFrameworkCore;
using VoiceConcierge.Api.Models;
using VoiceConcierge.Data;
using VoiceConcierge.Data.Enums;
using DataVoiceConfig = VoiceConcierge.Data.Entities.VoiceConfig;

namespace VoiceConcierge.Api.Repositories;

public class VoiceConfigRepository(ConciergeDbContext db) : IVoiceConfigRepository
{
    public async Task<VoiceConfigModel?> GetAsync(CancellationToken ct)
    {
        var entity = await db.VoiceConfig
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        return entity is null ? null : MapToModel(entity);
    }

    public async Task<VoiceConfigModel?> UpdateAsync(VoiceId newActiveVoiceId, CancellationToken ct)
    {
        var entity = await db.VoiceConfig
            .FirstOrDefaultAsync(x => x.Id == DataVoiceConfig.SingletonId, ct);
        if (entity is null)
            return null;

        entity.ActiveVoiceId = newActiveVoiceId;
        entity.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return MapToModel(entity);
    }

    private static VoiceConfigModel MapToModel(DataVoiceConfig entity) => new()
    {
        ActiveVoiceId = entity.ActiveVoiceId,
        UpdatedAt = entity.UpdatedAt,
    };
}
