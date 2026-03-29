# Tài liệu tích hợp OrderService

Tài liệu mô tả API tích hợp OrderService, tuân thủ theo chuẩn RestfulAPI và OpenAPI.

Cấu trúc bọc JSON cho mọi response (thành công / lỗi) được mô tả một lần tại [Phụ lục: Envelope response (body chung)](#response-body-envelope).

## 1. API tạo đơn hàng

Tạo đơn hàng mới ở trạng thái nháp / chờ thanh toán (theo quy ước nghiệp vụ). Không kiểm tra tồn kho (theo giả định SAD).

- **Method:** `POST`
- **Endpoint:** `/api/v1/orders`

### Request

- **Headers**
    - `Content-Type`: `application/json`
    - `Authorization`: `Bearer {access_token}`
    - `RequestId`: `{uuid}` — định danh request do client sinh, dùng correlation / support.
    - `IdempotentId`: `{uuid}` — idempotent theo **một lần tạo đơn**; gửi lại cùng giá trị khi retry phải trả cùng đơn (không tạo bản ghi trùng).

- **Body (JSON)**

| Trường | Kiểu | Bắt buộc | Mô tả |
|--------|------|----------|--------|
| `name` | string | Có | Tên đơn hàng (phục vụ tra cứu/lọc theo tên). |
| `date` | string (`date`) | Không | Ngày chứng từ/đơn theo calendar của client; mặc định server có thể dùng ngày UTC hiện tại nếu bỏ qua. |
| `totalAmount` | number (decimal) | Có | Tổng tiền hàng trước giảm giá; server có thể đối chiếu với tổng từ dòng. |
| `totalDiscountAmount` | number (decimal) | Có | Tổng giảm giá toàn đơn (≥ 0). |
| `totalPaymentAmount` | number (decimal) | Có | Số tiền thanh toán dự kiến (= `totalAmount` − `totalDiscountAmount` theo quy tắc làm tròn của hệ thống). |
| `items` | array | Có | Danh sách dòng hàng; ít nhất một phần tử. |

**Phần tử `items[]`:**

| Trường | Kiểu | Bắt buộc | Mô tả |
|--------|------|----------|--------|
| `productId` | string | Có | Định danh sản phẩm phía catalog / SKU. |
| `productName` | string | Có | Tên hiển thị tại thời điểm đặt (snapshot). |
| `unitPrice` | number (decimal) | Có | Đơn giá một đơn vị (≥ 0). |
| `quantity` | number (integer) | Có | Số lượng (> 0). |
| `amount` | number (decimal) | Có | Thành tiền dòng trước giảm (= `unitPrice` × `quantity` theo quy tắc làm tròn). |
| `discountAmount` | number (decimal) | Có | Giảm giá trên dòng (≥ 0). |
| `paymentAmount` | number (decimal) | Có | Thành tiền phải thu của dòng sau giảm. |

```json
{
    "name": "Đơn hàng demo — tháng 3",
    "date": "2026-03-29",
    "totalAmount": 1000000,
    "totalDiscountAmount": 50000,
    "totalPaymentAmount": 950000,
    "items": [
        {
            "productId": "prd_001",
            "productName": "Dịch vụ A",
            "unitPrice": 500000,
            "quantity": 2,
            "amount": 1000000,
            "discountAmount": 50000,
            "paymentAmount": 950000
        }
    ]
}
```

### Response
- **Headers:**
    - `RequestId`: echo `RequestId` từ request (nếu hợp lệ) hoặc id do gateway gán.
    - `TraceId`: `{uuid}` — id trace nội bộ / distributed tracing.

### Response thành công

- **Status:** `201 Created`

- **Body:** JSON bọc theo [envelope response chung](#response-body-envelope). Object `data` trong trường hợp thành công:

**Object `data`:**

| Trường | Kiểu | Mô tả |
|--------|------|--------|
| `id` | string (uuid) | Định danh đơn. |
| `name` | string | Tên đơn (snapshot). |
| `date` | string (`date`) | Ngày chứng từ lưu trữ. |
| `status` | string | Trạng thái đơn sau khi tạo (ví dụ `Draft`). |
| `createdAt` | string (`date-time`) | Thời điểm tạo (UTC, ISO 8601). |
| `createdBy` | string | Id người / principal tạo đơn (subject từ token). |
| `createdByName` | string | Tên hiển thị (nếu có). |
| `totalAmount` | number | Tổng tiền hàng (đã chuẩn hoá server). |
| `totalDiscountAmount` | number | Tổng giảm giá. |
| `totalPaymentAmount` | number | Tổng thanh toán dự kiến. |
| `items` | array | Dòng hàng đã lưu (cùng cấu trúc với request, có thể bổ sung `lineId` nếu server sinh). |

```json
{
    "code": "0000",
    "message": "Success",
    "errors": [],
    "data": {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Đơn hàng demo — tháng 3",
        "date": "2026-03-29",
        "status": "Draft",
        "createdAt": "2026-03-29T10:00:00.000Z",
        "createdBy": "sub_abc123",
        "createdByName": "Nguyễn Văn A",
        "totalAmount": 1000000,
        "totalDiscountAmount": 50000,
        "totalPaymentAmount": 950000,
        "items": [
            {
                "productId": "prd_001",
                "productName": "Dịch vụ A",
                "unitPrice": 500000,
                "quantity": 2,
                "amount": 1000000,
                "discountAmount": 50000,
                "paymentAmount": 950000
            }
        ]
    }
}
```

### Response lỗi

- **Headers:** giống thành công (`RequestId`, `TraceId` khi có).
- **HTTP status (minh hoạ):**
    - **400** — JSON sai cấu trúc.
    - **401** — Thiếu / sai Bearer token.
    - **403** — Không đủ quyền tạo đơn trong ngữ cảnh tenant.
    - **409** — Nếu trùng IdempotentId và đã tạo trc đó
    - **422** — Dữ liệu hợp lệ cú pháp nhưng sai business logic
    - **500** — Lỗi ko xác định

- **Body:** cùng [envelope](#response-body-envelope) với biến thể lỗi (xem phụ lục).

```json
{
    "code": "4220",
    "message": "Order totals do not match line items.",
    "errors": [
        {
            "fieldName": "totalPaymentAmount",
            "message": "Must equal sum of line paymentAmount within tolerance."
        },
        {
            "fieldName": "items[0].quantity",
            "message": "Must be greater than zero."
        }
    ]
}
```

## 2. API checkout đơn hàng

Thực hiện checkout / thanh toán cho đơn đã tồn tại. URL redirect/callback và metadata giao dịch do OrderService và cấu hình tenant quyết định. Client chỉ định kênh qua `paymentServiceId` và có thể chọn phương thức qua `paymentMethod` khi gateway hỗ trợ nhiều lựa chọn.

- **Method:** `POST`
- **Endpoint:** `/api/v1/orders/{id}/checkout`

### Request

- **Path**

| Tham số | Kiểu | Bắt buộc | Mô tả |
|---------|------|----------|--------|
| `id` | string (uuid) | Có | Định danh đơn hàng cần checkout. |

- **Headers**

    - `Content-Type`: `application/json`
    - `Authorization`: `Bearer {access_token}`
    - `RequestId`: `{uuid}` — định danh request do client sinh, dùng correlation / support.
    - `IdempotentId`: `{uuid}` — idempotent theo **một ý định thanh toán**; retry cùng giá trị phải trả cùng kết quả nghiệp vụ (không tạo giao dịch trùng).

- **Body (JSON)**

| Trường | Kiểu | Bắt buộc | Mô tả |
|--------|------|----------|--------|
| `paymentServiceId` | string | Có | Mã kênh / cấu hình thanh toán tích hợp với Payment Gateway. |
| `paymentMethod` | string | Không | Phương thức thanh toán (ví dụ `card`, `bank_transfer`, `ewallet`); bắt buộc nếu quy ước tích hợp gateway yêu cầu client chọn phương thức trước khi charge/redirect. |

```json
{
    "paymentServiceId": "pay_svc_live_01",
    "paymentMethod": "card"
}
```

### Response

- **Headers:**
    - `RequestId`: echo `RequestId` từ request (nếu hợp lệ) hoặc id do gateway gán.
    - `TraceId`: `{uuid}` — id trace nội bộ / distributed tracing.

### Response thành công

- **Status:** `200 OK` — thanh toán được xác nhận trong cùng phiên request (kết quả final từ gateway trong round-trip hiện tại).

- **Body:** JSON bọc theo [envelope response chung](#response-body-envelope). Object `data` khi thành công:

**Object `data`:**

| Trường | Kiểu | Mô tả |
|--------|------|--------|
| `orderId` | string (uuid) | Trùng `id` trên path. |
| `status` | string | Trạng thái đơn sau checkout (ví dụ `Paid`, `PendingPayment`). |
| `paymentTransactionId` | string | Mã giao dịch do Payment Gateway trả về (nếu đã có). |
| `paymentAt` | string (`date-time`) | Thời điểm xác nhận thanh toán (UTC, ISO 8601); có thể `null` nếu luồng bất đồng bộ. |
| `paymentBy` | string | Id chủ thể / principal thực hiện thanh toán (subject từ token). |
| `paymentByName` | string | Tên hiển thị (nếu có). |

```json
{
    "code": "0000",
    "message": "Success",
    "errors": [],
    "data": {
        "orderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "status": "Paid",
        "paymentTransactionId": "gw_txn_abc123",
        "paymentAt": "2026-03-29T10:15:30.000Z",
        "paymentBy": "sub_abc123",
        "paymentByName": "Nguyễn Văn A"
    }
}
```

### Response lỗi

- **Headers:** giống thành công (`RequestId`, `TraceId` khi có).
- **HTTP status (minh hoạ):**
    - **400** — JSON sai cấu trúc hoặc thiếu trường bắt buộc.
    - **401** — Thiếu / sai Bearer token.
    - **403** — Không đủ quyền checkout đơn này.
    - **404** — Không có đơn với `id` hoặc đơn không thuộc ngữ cảnh người gọi.
    - **409** — Đơn không ở trạng thái cho phép checkout (đã thanh toán, đã huỷ, …) hoặc xung đột idempotent.
    - **422** — Gateway / nghiệp vụ từ chối thanh toán (từ chối thẻ, hết hạn, …).
    - **500** — Lỗi máy chủ.

- **Body:** cùng [envelope](#response-body-envelope) với biến thể lỗi (xem phụ lục).

```json
{
    "code": "4221",
    "message": "Payment was declined.",
    "errors": [
        {
            "fieldName": "payment",
            "message": "INSUFFICIENT_FUNDS"
        }
    ]
}
```

## 3. API tra cứu đơn hàng

Tra cứu danh sách đơn hàng có phân trang, lọc theo tên đơn (contains / prefix tuỳ quy ước triển khai).

- **Method:** `GET`
- **Endpoint:** `/api/v1/orders`

### Request

- **Headers**

    - `Authorization`: `Bearer {access_token}`
    - `RequestId`: `{uuid}` — định danh request do client sinh, dùng correlation / support.

- **Query**

| Tham số | Kiểu | Bắt buộc | Mô tả |
|---------|------|----------|--------|
| `name` | string | Không | Chuỗi tìm theo tên đơn (lọc mờ theo SAD); bỏ qua nếu rỗng thì trả theo quy ước mặc định (ví dụ đơn mới nhất). |
| `page` | integer | Không | Số trang (bắt đầu từ `0` hoặc `1` tuỳ quy ước API; mặc định nên ghi rõ trong OpenAPI). |
| `size` | integer | Không | Số bản ghi mỗi trang (có giới hạn tối đa server). |

### Response

- **Headers:**
    - `RequestId`: echo `RequestId` từ request (nếu hợp lệ) hoặc id do gateway gán.
    - `TraceId`: `{uuid}` — id trace nội bộ / distributed tracing.

### Response thành công

- **Status:** `200 OK`

- **Body:** JSON bọc theo [envelope response chung](#response-body-envelope). Object `data` khi thành công:

**Object `data`:**

| Trường | Kiểu | Mô tả |
|--------|------|--------|
| `totalItems` | integer | Tổng số bản ghi thỏa điều kiện lọc. |
| `totalPages` | integer | Tổng số trang. |
| `page` | integer | Trang hiện tại. |
| `size` | integer | Kích thước trang thực tế. |
| `items` | array | Danh sách đơn (tóm tắt). |

**Phần tử `items[]`:**

| Trường | Kiểu | Mô tả |
|--------|------|--------|
| `id` | string (uuid) | Định danh đơn. |
| `name` | string | Tên đơn. |
| `date` | string (`date`) | Ngày chứng từ. |
| `status` | string | Trạng thái đơn. |
| `createdAt` | string (`date-time`) | Thời điểm tạo (UTC). |
| `createdBy` | string | Id người tạo. |
| `createdByName` | string | Tên hiển thị người tạo. |
| `totalAmount` | number | Tổng tiền hàng. |
| `totalDiscountAmount` | number | Tổng giảm giá. |
| `totalPaymentAmount` | number | Tổng thanh toán dự kiến. |

```json
{
    "code": "0000",
    "message": "Success",
    "errors": [],
    "data": {
        "totalItems": 2,
        "totalPages": 1,
        "page": 0,
        "size": 20,
        "items": [
            {
                "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                "name": "Đơn hàng demo — tháng 3",
                "date": "2026-03-29",
                "status": "Draft",
                "createdAt": "2026-03-29T10:00:00.000Z",
                "createdBy": "sub_abc123",
                "createdByName": "Nguyễn Văn A",
                "totalAmount": 1000000,
                "totalDiscountAmount": 50000,
                "totalPaymentAmount": 950000
            },
            {
                "id": "8b2c1a00-1111-4222-8333-444455556666",
                "name": "Đơn hàng bổ sung",
                "date": "2026-03-28",
                "status": "Paid",
                "createdAt": "2026-03-28T09:30:00.000Z",
                "createdBy": "sub_abc123",
                "createdByName": "Nguyễn Văn A",
                "totalAmount": 500000,
                "totalDiscountAmount": 0,
                "totalPaymentAmount": 500000
            }
        ]
    }
}
```

### Response lỗi

- **Headers:** giống thành công (`RequestId`, `TraceId` khi có).
- **HTTP status (minh hoạ):**
    - **400** — Query không hợp lệ (`page` / `size` âm, vượt max, …).
    - **401** — Thiếu / sai Bearer token.
    - **403** — Không đủ quyền tra cứu trong ngữ cảnh tenant.
    - **422** — Tham số hợp lệ cú pháp nhưng vi phạm quy tắc (ví dụ `size` vượt ngưỡng).
    - **500** — Lỗi máy chủ.

- **Body:** cùng [envelope](#response-body-envelope) với biến thể lỗi (xem phụ lục).

```json
{
    "code": "4001",
    "message": "Invalid query parameters.",
    "errors": [
        {
            "fieldName": "size",
            "message": "Must not exceed 100."
        }
    ]
}
```

<a id="response-body-envelope"></a>

## Phụ lục: Envelope response (body chung)

Mọi response JSON của API (trừ khi ghi chú riêng) dùng cùng lớp bọc ở root.

### Cấu trúc root

| Trường | Kiểu | Mô tả |
|--------|------|--------|
| `code` | string | Mã nghiệp vụ; thành công: `"0000"`. |
| `message` | string | Thông điệp ngắn (có thể rỗng khi thành công). |
| `errors` | array | Khi thành công: `[]` (hoặc bỏ trường nếu schema OpenAPI cho phép). Khi lỗi: danh sách chi tiết (xem dưới). |
| `data` | object \| null | Payload nghiệp vụ **tuỳ từng endpoint** (mô tả trong mục API tương ứng). Khi lỗi: `null` hoặc bỏ trường. |

### Biến thể thành công

- `code` = `"0000"`.
- `errors` = `[]` hoặc không gửi trường `errors`.
- `data` là object theo schema của API đó.

### Biến thể lỗi

- `code` khác `"0000"` (mã lỗi nghiệp vụ do dịch vụ quy định).
- `message` mô tả ngắn gọn cho người đọc / client.
- `errors` chứa một hoặc nhiều phần tử lỗi theo field (có thể rỗng nếu chỉ dùng `message`).
- `data` = `null` hoặc không gửi trường `data`.

**Phần tử `errors[]`:**

| Trường | Kiểu | Mô tả |
|--------|------|--------|
| `fieldName` | string | Tên field / tham số liên quan (có thể dạng `items[0].quantity`). |
| `message` | string | Mô tả lỗi cục bộ hoặc mã lỗi máy đọc được. |
