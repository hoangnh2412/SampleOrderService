using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;
using OrderService.Domain.Shared.Enums;

namespace OrderService.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository(OrderDbContext db) : IOrderRepository
{
    public async Task<Order?> GetByIdempotentIdWithDetailsAsync(
        string idempotentId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idempotentId))
            return null;

        var key = idempotentId.Trim();
        return await db.Orders.AsNoTracking()
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.IdempotentId == key, cancellationToken);
    }

    public async Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await db.Orders.AddAsync(order, cancellationToken);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException) when (!string.IsNullOrWhiteSpace(order.IdempotentId))
        {
            db.ChangeTracker.Clear();
            var winner = await db.Orders.AsNoTracking()
                .Include(o => o.Details)
                .FirstOrDefaultAsync(o => o.IdempotentId == order.IdempotentId.Trim(), cancellationToken);
            if (winner is not null)
            {
                order.ClearDomainEvents();
                return winner;
            }

            throw;
        }

        return order;
    }

    public async Task<Order?> LoadByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Orders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Order?> GetByIdWithDetailsReadOnlyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Orders
            .AsNoTracking()
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddPaymentHistoryAsync(PaymentHistory history, CancellationToken cancellationToken = default)
    {
        await db.PaymentHistories.AddAsync(history, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<PaymentHistory?> GetPaymentHistoryByEntityIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await db.PaymentHistories.AsNoTracking()
            .Where(h => h.EntityId == orderId && h.EntityType == PaymentHistoryEntityType.Order)
            .OrderByDescending(h => h.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PaymentHistory?> GetPaymentHistoryByTransactionIdAsync(
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
            return null;

        var key = transactionId.Trim();
        return await db.PaymentHistories.AsNoTracking()
            .Where(h => h.TransactionId == key && h.EntityType == PaymentHistoryEntityType.Order)
            .OrderByDescending(h => h.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Order> Items, int TotalCount)> SearchPagedAsync(
        string? nameFragment,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = db.Orders.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nameFragment))
            query = query.Where(o => o.Code.Contains(nameFragment));

        query = query.OrderByDescending(o => o.CreatedAtUtc);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
