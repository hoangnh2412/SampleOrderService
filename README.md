# OrderService

Dự án mẫu **dịch vụ đơn hàng** (.NET): khách tạo đơn, tra cứu theo tên, checkout thanh toán; sau khi thanh toán thành công, hệ thống phối hợp xuất hóa đơn, đẩy đơn sang Production nội bộ và gửi email thông báo.

Luồng xử lý tích hợp dùng **outbox / message broker** (demo dùng broker in-memory) để tách phần đồng bộ HTTP và phần hậu xử lý bất đồng bộ.

## Tài liệu

- **[Software Architecture Document (SAD)](docs/sad.md)** — yêu cầu, quyết định kiến trúc (C4, component, ERD), hệ quả và phương án đã xem xét.

## Công nghệ (tóm tắt)

Backend hướng tới **ASP.NET Core**, **Entity Framework**, SQLite; cache và message broker in-memory cho môi trường demo; frontend tham chiếu **React**. Chi tiết stack và sơ đồ nằm trong SAD.

## Repo

| Đường dẫn | Nội dung |
|-----------|----------|
| `docs/sad.md` | Kiến trúc & thiết kế |
| `OrderService.sln` | Solution Visual Studio |
| `skills/architechture-dotnet.md` | Ghi chú / skill kiến trúc .NET (nếu dùng nội bộ) |
