# OrderService

Dự án mẫu OrderService (.NET) với nghiệp vụ cơ bản: Khách tạo đơn, tra cứu theo tên, checkout thanh toán; sau khi thanh toán thành công, hệ thống phối hợp xuất hóa đơn, đẩy đơn sang Production nội bộ và gửi email thông báo.

## Tài liệu

- **[Software Architecture Document (SAD)](docs/sad.md)** — yêu cầu, quyết định kiến trúc (C4, component, ERD), hệ quả và phương án đã xem xét.

## Công nghệ (tóm tắt)

1. ASP.NET Core
2. Entity Framework
3. SQLite
4. Cache inmemory
5. message broker in-memory
6. JWT

