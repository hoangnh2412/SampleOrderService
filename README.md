# OrderService

Dự án mẫu OrderService (.NET) với nghiệp vụ cơ bản: Khách tạo đơn, tra cứu theo tên, checkout thanh toán; sau khi thanh toán thành công, hệ thống phối hợp xuất hóa đơn, đẩy đơn sang Production nội bộ và gửi email thông báo.

## Tài liệu

- **[Software Architecture Document (SAD)](docs/sad.md)** — yêu cầu, quyết định kiến trúc (C4, component, ERD), hệ quả và phương án đã xem xét.

- **[API spec](docs/openapi.yml)** - Tài liệu đặc tả API: Tạo token, tạo đơn hàng, checkout đơn hàng, webhook nhận kết quả thanh toán

## Công nghệ (tóm tắt)

1. ASP.NET Core
2. Entity Framework
3. SQLite
4. Cache inmemory
5. message broker in-memory
6. JWT

## Hướng dẫn

1. **Yêu cầu**: [.NET SDK](https://dotnet.microsoft.com/download) (phiên bản tương thích với các project trong solution).

2. **Chạy API** (từ thư mục gốc repo):

   ```bash
   cd src/OrderService.Host
   dotnet run
   ```

   Mặc định API lắng nghe **http://localhost:5048** (profile `https` dùng thêm cổng HTTPS theo `launchSettings.json`).

3. **Swagger UI** (chỉ bật khi `ASPNETCORE_ENVIRONMENT=Development`): mở [http://localhost:5048/swagger](http://localhost:5048/swagger). Có thể lấy JWT qua endpoint token rồi thử các API trong Swagger.

4. **Cơ sở dữ liệu**: SQLite (`orders.db`) được tạo tự động lần đầu chạy (EF `EnsureCreated`), file nằm cùng thư mục làm việc khi chạy Host (thường là `src/OrderService.Host/`).

5. **Cấu hình tối thiểu**: JWT và webhook thanh toán mẫu nằm trong `src/OrderService.Host/appsettings.json`; đổi khi triển khai thật.