using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Repositories;
using OrderService.Domain.Entities;
using OrderService.Domain.Shared.Enums;

namespace OrderService.Infrastructure.Persistence.Repositories;

public sealed class OutboxMessageRepository(OrderDbContext db) : IOutboxMessageRepository
{
    public async Task<IReadOnlyList<OutboxMessage>> FetchMessagesAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        return await db.OutboxMessages
            .AsNoTracking()
            .Where(m => m.Status == OrderMessageStatus.New)
            .OrderBy(m => m.CreatedAtUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var row = await db.OutboxMessages.FindAsync([messageId], cancellationToken);
        if (row is null)
            return;
        row.MarkAsProcessed();
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkFailedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var row = await db.OutboxMessages.FindAsync([messageId], cancellationToken);
        if (row is null)
            return;
        row.MarkAsFailed();
        await db.SaveChangesAsync(cancellationToken);
    }
}
