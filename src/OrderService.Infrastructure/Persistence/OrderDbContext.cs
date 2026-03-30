using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Outbox;
using OrderService.Infrastructure.Persistence.Configurations;

namespace OrderService.Infrastructure.Persistence;

public sealed class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<PaymentHistory> PaymentHistories => Set<PaymentHistory>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderDetailConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess) =>
        ExecuteSaveWithOutbox(acceptAllChangesOnSuccess);

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) =>
        ExecuteSaveWithOutboxAsync(acceptAllChangesOnSuccess, cancellationToken);

    /// <summary>
    /// Stage outbox → persist → clear events; lỗi thì detach outbox đã stage rồi ném lại.
    /// </summary>
    private int ExecuteSaveWithOutbox(bool acceptAllChangesOnSuccess)
    {
        var staging = StageOutboxFromTrackedAggregates();
        try
        {
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            ClearDomainEventsFor(staging.Aggregates);
            return result;
        }
        catch
        {
            DetachStagedOutboxMessages(staging.Messages);
            throw;
        }
    }

    private async Task<int> ExecuteSaveWithOutboxAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
    {
        var staging = StageOutboxFromTrackedAggregates();
        try
        {
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);
            ClearDomainEventsFor(staging.Aggregates);
            return result;
        }
        catch
        {
            DetachStagedOutboxMessages(staging.Messages);
            throw;
        }
    }

    /// <summary>
    /// Chuẩn bị bản ghi outbox cho mọi aggregate đang được EF theo dõi và sắp ghi xuống DB.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Theo transactional outbox (SAD): domain event phải được lưu trong <strong>cùng transaction</strong> với thay đổi aggregate,
    /// để không bao giờ commit đơn mà mất event (hoặc ngược lại). Vì vậy, ngay trước <c>SaveChanges</c>, ta quét
    /// <see cref="ChangeTracker"/> tìm các thực thể vừa là <see cref="IAggregateRoot"/> vừa ở trạng thái <see cref="EntityState.Added"/>
    /// hoặc <see cref="EntityState.Modified"/> — tức là sẽ tham gia lần ghi này. Với mỗi aggregate còn event trong bộ nhớ,
    /// serialize từng event và thêm một dòng <see cref="OutboxMessage"/> vào context; lần gọi <c>base.SaveChanges</c> sau đó
    /// sẽ insert/update aggregate và outbox cùng lúc.
    /// </para>
    /// <para>
    /// <see cref="OutboxStagingResult.Aggregates"/>: các aggregate đã được stage outbox trong lần gọi này; sau khi lưu
    /// thành công cần <see cref="IAggregateRoot.ClearDomainEvents"/> để tránh ghi trùng outbox ở lần <c>SaveChanges</c> kế tiếp.
    /// </para>
    /// <para>
    /// <see cref="OutboxStagingResult.Messages"/>: các <see cref="OutboxMessage"/> vừa <c>Add</c> vào context trong bước stage.
    /// Nếu <c>base.SaveChanges</c> thất bại, phải <see cref="DetachStagedOutboxMessages"/> để gỡ chúng khỏi tracker; nếu không,
    /// lần retry có thể stage thêm bộ outbox mới cho cùng các event và gây trùng lặp hoặc trạng thái tracker lẫn lộn.
    /// </para>
    /// </remarks>
    /// <returns>Kết quả stage: danh sách aggregate cần xóa event sau khi lưu thành công, và danh sách bản ghi outbox vừa gắn vào context.</returns>
    private OutboxStagingResult StageOutboxFromTrackedAggregates()
    {
        var aggregates = new List<IAggregateRoot>();
        var messages = new List<OutboxMessage>();

        // Snapshot: Add(OutboxMessage) thay đổi ChangeTracker — không được enumerate trực tiếp khi đang Add.
        foreach (var entry in ChangeTracker.Entries().ToList())
        {
            if (entry.Entity is not IAggregateRoot aggregate)
                continue;
            if (entry.State is not (EntityState.Added or EntityState.Modified))
                continue;

            var domainEvents = aggregate.GetDomainEvents().ToList();
            if (domainEvents.Count == 0)
                continue;

            aggregates.Add(aggregate);
            foreach (var domainEvent in domainEvents)
            {
                var payload = DomainEventOutboxSerializer.Serialize(domainEvent);
                var message = OutboxMessage.Create(domainEvent.EntityId, payload, domainEvent.CreatedAtUtc);
                OutboxMessages.Add(message);
                messages.Add(message);
            }
        }

        return new OutboxStagingResult(aggregates, messages);
    }

    /// <summary>
    /// Kết quả của bước stage outbox trước <c>SaveChanges</c> (không dùng tuple để tên thuộc tính rõ ràng khi gọi).
    /// </summary>
    private sealed record OutboxStagingResult(IReadOnlyList<IAggregateRoot> Aggregates, IReadOnlyList<OutboxMessage> Messages);

    private void DetachStagedOutboxMessages(IReadOnlyList<OutboxMessage> messages)
    {
        foreach (var message in messages)
        {
            var entry = Entry(message);
            if (entry.State != EntityState.Detached)
                entry.State = EntityState.Detached;
        }
    }

    private static void ClearDomainEventsFor(IReadOnlyList<IAggregateRoot> aggregates)
    {
        foreach (var aggregate in aggregates)
            aggregate.ClearDomainEvents();
    }
}
