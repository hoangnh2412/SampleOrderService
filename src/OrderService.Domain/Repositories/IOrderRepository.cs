using OrderService.Domain.Entities;

namespace OrderService.Domain.Repositories;

/// <summary>
/// Port truy cập persistence cho aggregate <see cref="Order"/> (và bản ghi liên quan như <see cref="PaymentHistory"/>).
/// Application layer chỉ phụ thuộc interface này, không phụ thuộc EF hay SQLite.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Tra cứu đơn theo <see cref="Order.IdempotentId"/> (read-only, kèm <see cref="Order.Details"/>).
    /// Dùng ở tầng Application cho chính sách idempotent trước khi tạo aggregate mới.
    /// </summary>
    Task<Order?> GetByIdempotentIdWithDetailsAsync(
        string idempotentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lưu đơn mới. Trường hợp hai request đồng thời cùng <see cref="Order.IdempotentId"/> (sau khi Application đã kiểm tra),
    /// có thể bắt vi phạm unique index và trả về đơn đã persist — xem triển khai Infrastructure.
    /// </summary>
    /// <param name="order">Đơn đã được domain tạo (có <see cref="Order.Details"/> trong bộ nhớ).</param>
    /// <param name="cancellationToken">Token hủy tác vụ.</param>
    /// <returns>Đơn vừa lưu, hoặc đơn đã tồn tại nếu xảy ra xung đột đồng thời trên cùng khóa idempotent.</returns>
    Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tải đơn theo id để chỉnh sửa (tracking): kèm <see cref="Order.Details"/> cho cập nhật trạng thái / nghiệp vụ cần graph đầy đủ.
    /// </summary>
    /// <param name="id">Định danh đơn.</param>
    /// <param name="cancellationToken">Token hủy tác vụ.</param>
    /// <returns>Thực thể được DbContext theo dõi, hoặc <see langword="null"/> nếu không có đơn.</returns>
    Task<Order?> LoadByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Đọc đơn theo id chỉ để truy vấn (no-tracking), kèm toàn bộ dòng chi tiết — dùng cho API trả DTO, không chỉnh sửa.
    /// </summary>
    /// <param name="id">Định danh đơn.</param>
    /// <param name="cancellationToken">Token hủy tác vụ.</param>
    /// <returns>Bản snapshot đơn + details, hoặc <see langword="null"/> nếu không tồn tại.</returns>
    Task<Order?> GetByIdWithDetailsReadOnlyAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ghi các thay đổi trên đơn đang được tracking (ví dụ sau checkout / cập nhật trạng thái trên aggregate <see cref="Order"/>).
    /// </summary>
    /// <param name="order">Cùng instance đã lấy từ <see cref="LoadByIdAsync"/>.</param>
    /// <param name="cancellationToken">Token hủy tác vụ.</param>
    Task SaveAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Thêm một bản ghi lịch sử thanh toán (ERD <c>PaymentHistory</c>), thường gọi sau khi checkout thành công.
    /// </summary>
    /// <param name="history">Bản ghi do domain factory tạo.</param>
    /// <param name="cancellationToken">Token hủy tác vụ.</param>
    Task AddPaymentHistoryAsync(PaymentHistory history, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy bản ghi thanh toán gần nhất cho đơn (<see cref="PaymentHistory.EntityType"/> = Order), dùng khi replay idempotent checkout.
    /// </summary>
    Task<PaymentHistory?> GetPaymentHistoryByEntityIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tra cứu lịch sử thanh toán theo mã giao dịch cổng (idempotency webhook).
    /// </summary>
    Task<PaymentHistory?> GetPaymentHistoryByTransactionIdAsync(
        string transactionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Tra cứu danh sách đơn có phân trang, lọc theo chuỗi con của mã/tên đơn (map OpenAPI <c>name</c> → <see cref="Order.Code"/>).
    /// </summary>
    /// <param name="nameFragment">Chuỗi lọc; <see langword="null"/> hoặc rỗng thì không lọc theo tên.</param>
    /// <param name="page">Chỉ số trang (0-based).</param>
    /// <param name="pageSize">Số bản ghi mỗi trang.</param>
    /// <param name="cancellationToken">Token hủy tác vụ.</param>
    /// <returns>Danh sách đơn (snapshot, không kèm details) và tổng số bản ghi khớp điều kiện (trước khi phân trang).</returns>
    Task<(IReadOnlyList<Order> Items, int TotalCount)> SearchPagedAsync(
        string? nameFragment,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
