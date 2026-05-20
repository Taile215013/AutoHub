# Auto Hub Project - Domain Entities & Database Schema
tất cả khi thêm vào database của sql server, phải dùng tiếng anh, vì tiếng việt có thể gây lỗi font hoặc ký tự đặc biệt
tất cả các dữ liệu thêm vào , có thể thêm tiếng việt, nhưng phải tuân theo quy tắc thêm vào database

## 1. Hệ Thống Danh Mục Tổng (Master Data & Enums)
Để quản lý các loại xe (loại xe, loại nhiên liệu, loại truyền động), hệ thống sử dụng các chuỗi chuẩn (String/NVARCHAR) hoặc các bảng danh mục để phân loại.

- **VehicleType:** `Motorbike` (Xe máy), `Auto` (Ô tô)
- **FuelType:** `Gasoline` (Xăng), `Electric` (Điện)
- **TransmissionType:** `Automatic` (Tay ga/Số tự động), `Manual` (Số cơ/Số sàn), `Clutch` (Xe côn tay)

---

## 2. Chi Tiết Các Đối Tượng & Bảng Dữ Liệu (Entities & Tables)

### 2.1. Đối tượng: Country (Quốc gia)
*Mối quan hệ: Một quốc gia có thể có nhiều thương hiệu (Brands).*
- **Tên bảng SQL:** `Countries`

| Tên Thuộc Tính (C# & SQL) | Kiểu C# | Kiểu SQL | Ràng buộc | Ghi chú |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | `int` | `INT` | PRIMARY KEY, IDENTITY | Mã quốc gia |
| `Name` | `string` | `NVARCHAR(100)` | NOT NULL | Tên quốc gia (Nhật, Đức, Mỹ...) |

### 2.2. Đối tượng: Brand (Thương hiệu)
*Mối quan hệ: Thuộc về 1 quốc gia. Phân loại rõ ràng thương hiệu xe, phụ tùng hay đồ chơi.*
- **Tên bảng SQL:** `Brands`

| Tên Thuộc Tính | Kiểu C# | Kiểu SQL | Ràng buộc | Ghi chú |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | `int` | `INT` | PRIMARY KEY, IDENTITY | Mã thương hiệu |
| `Name` | `string` | `NVARCHAR(100)` | NOT NULL | Tên thương hiệu (Honda, Brembo...) |
| `CountryId` | `int` | `INT` | FOREIGN KEY -> Countries(Id) | Thuộc quốc gia nào |
| `IsVehicleBrand` | `bool` | `BIT` | NOT NULL (Default: 0) | Có sản xuất xe không? |
| `IsPartBrand` | `bool` | `BIT` | NOT NULL (Default: 0) | Có sản xuất phụ tùng không? |
| `IsToyBrand` | `bool` | `BIT` | NOT NULL (Default: 0) | Có sản xuất đồ chơi xe không? |

### 2.3. Đối tượng: Vehicle (Bán Xe)
*Mô tả: Thông tin chi tiết về xe để bán. Số lượng có thể NULL (hàng sắp về, chưa có sẵn).*
- **Tên bảng SQL:** `Vehicles`

| Tên Thuộc Tính | Kiểu C# | Kiểu SQL | Ràng buộc | Ghi chú |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | `int` | `INT` | PRIMARY KEY, IDENTITY | Mã xe |
| `Name` | `string` | `NVARCHAR(150)` | NOT NULL | Tên xe (Civic, Exciter...) |
| `BrandId` | `int` | `INT` | FOREIGN KEY -> Brands(Id) | Thuộc hãng nào |
| `VehicleType` | `string` | `VARCHAR(20)` | NOT NULL | `Motorbike` hoặc `Auto` |
| `FuelType` | `string` | `VARCHAR(20)` | NOT NULL | `Gasoline` (Xăng) hoặc `Electric` (Điện) |
| `Transmission` | `string` | `VARCHAR(20)` | NOT NULL | `Automatic`, `Manual`, `Clutch` |
| `Price` | `decimal` | `DECIMAL(18,2)`| NOT NULL | Giá bán xe |
| `Quantity` | `int?` | `INT` | **NULLABLE** (Cho phép NULL) | Số lượng trong kho (Null = Hết/Sắp về) |
| `EngineType` | `string` | `VARCHAR(50)` | NOT NULL | Loại động cơ (Ví dụ: `I4`, `V6`, `Xăng 4 thì`, `Điện AC Synchronous`) |
| `EngineCapacity` | `double?` | `FLOAT` | NULLABLE | Dung tích động cơ: Ô tô tính bằng Lít (2.0, 1.5), Xe máy tính bằng CC (150, 110). Xe điện để NULL. |
| `SeatingCapacity`| `int` | `INT` | NOT NULL (Default: 1) | Số lượng chỗ ngồi (Xe máy: 2, Ô tô: 4, 5, 7) |
| `Weight` | `double` | `FLOAT` | có thể null | Cân nặng của xe (kg) |
| `BodyStyle` | `string` | `NVARCHAR(50)` | có thể null | Dáng xe (Phân loại rõ ràng ở quy tắc bên dưới) |
#### 🛑 Quy tắc chuẩn hóa dữ liệu cho thuộc tính `BodyStyle` (Dáng xe):
Khi AI sinh code hoặc validate dữ liệu đầu vào, thuộc tính `BodyStyle` bắt buộc phải tuân theo logic của `VehicleType`:
- **Nếu `VehicleType` = `Auto` (4 bánh):** `BodyStyle` admin có thể thêm mới vì, tùy hãng sẽ có cách gọi dáng xe khác nhau, xử lý UI/UX sau này khi scoll và chọn sau
- **Nếu `VehicleType` = `Motorbike` (2 bánh):** `BodyStyle` như trên

### 2.3.1. Đối tượng: VehicleColor (Quản lý đa màu sắc cho xe)
*Mối quan hệ: Một chiếc xe có thể có từ 2-3 màu sắc trở lên. Đây là bảng trung gian dùng chung cho tất cả các loại xe (2 bánh và 4 bánh) để UI xử lý danh sách màu sau này.*
mặc định chỉ có 7 màu cơ bản, nếu xe có 2-3 màu trở lên, khi thêm màu mới sẽ thêm lần lượt
ví dụ admin chọn màu đỏ  nhấn ok, sau đó admin chọn tiếp màu xanh nhấn ok, khi thêm sản phẩm mới db sẽ là "Đỏ-Xanh"
- **Tên bảng SQL:** `VehicleColors`

| Tên Thuộc Tính | Kiểu C# | Kiểu SQL | Ràng buộc | Ghi chú |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | `int` | `INT` | PRIMARY KEY, IDENTITY | Mã định danh |
| `VehicleId` | `int` | `INT` | FOREIGN KEY -> Vehicles(Id) | Thuộc về chiếc xe nào |
| `ColorName` | `string` | `NVARCHAR(30)` | NOT NULL | Tên màu sắc |

#### 🛑 Quy tắc lưu trữ dữ liệu màu sắc (Bắt buộc cho AI khi viết code):

- **Quy chuẩn thiết kế:** Tuân thủ dạng chuẩn 1NF, **KHÔNG GOM NHIỀU MÀU VÀO MỘT Ô** (Không lưu kiểu `Đỏ, Xanh` vào cùng 1 dòng). Nếu một chiếc xe có nhiều màu độc lập, phải lưu thành các dòng (Rows) riêng biệt trong Database.
để sau này người dùng có tìm tiêu chí nhiều màu để tìm xe một cách chính xác 
- **Quy chuẩn chuỗi:** Lưu chữ **CÓ DẤU, CÓ KHOẢNG TRẮNG** bình thường (Ví dụ: `Đỏ`, `Xanh`, `Trắng-Đen`, `Xám Xi Măng`) để phục vụ hiển thị trực tiếp lên UI mà không cần format lại ở tầng C#. Không dùng dạng slug (`do_xanh`, `do-xanh`).
### 2.4. Đối tượng: SparePart (Phụ tùng) & Toy (Đồ chơi xe)
*Mối quan hệ: Một Brand có thể có nhiều phụ tùng/đồ chơi. Phân loại theo nhiều danh mục nhỏ.*
- **Tên bảng SQL:** `SpareParts`

| Tên Thuộc Tính | Kiểu C# | Kiểu SQL | Ràng buộc | Ghi chú |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | `int` | `INT` | PRIMARY KEY, IDENTITY | Mã phụ tùng |
| `Name` | `string` | `NVARCHAR(150)` | NOT NULL | Tên (Má phanh, Phuộc, Đèn LED...) |
| `BrandId` | `int` | `INT` | FOREIGN KEY -> Brands(Id) | Hãng sản xuất phụ tùng |
| `Category` | `string` | `NVARCHAR(50)` | NOT NULL | Phân loại: `Engine`, `Brake`, `Toy` (Đồ chơi)... |
| `Price` | `decimal` | `DECIMAL(18,2)`| NOT NULL | Giá bán |
| `StockQuantity`| `int` | `INT` | NOT NULL | Số lượng còn trong kho |

### 2.5. Đối tượng: Service (Dịch vụ về xe)
*Mô tả: Danh mục dịch vụ linh hoạt, cửa hàng có thể tự tạo mới dịch vụ theo nhu cầu.*
- **Tên bảng SQL:** `Services`

| Tên Thuộc Tính | Kiểu C# | Kiểu SQL | Ràng buộc | Ghi chú |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | `int` | `INT` | PRIMARY KEY, IDENTITY | Mã dịch vụ |
| `ServiceName` | `string` | `NVARCHAR(150)` | NOT NULL | Tên (Rửa xe, Thay nhớt, Độ phuộc...) |
| `BasePrice` | `decimal` | `DECIMAL(18,2)`| NOT NULL | Giá dịch vụ cơ bản |
| `IsActive` | `bool` | `BIT` | NOT NULL (Default: 1) | Dịch vụ còn phục vụ không |

---

## 3. Gợi ý mở rộng Hệ thống (Gợi ý thêm để làm phong phú Đồ án)
Để dự án "Auto Hub" thực tế và logic chặt chẽ hơn, AI Agent sẽ tự động tạo thêm các bảng liên kết sau khi xử lý nghiệp vụ:

### 3.1. Đối tượng: Order (Hóa đơn bán hàng tổng hợp)
*Mô tả: Khi khách hàng mua xe, mua phụ tùng hoặc làm dịch vụ.*
- **Tên bảng SQL:** `Orders`
- **Các thuộc tính chính:** `Id (int)`, `CustomerName (string)`, `OrderDate (DateTime)`, `TotalAmount (decimal)`.

### 3.2. Đối tượng: OrderDetail (Chi tiết hóa đơn)
- **Tên bảng SQL:** `OrderDetails`
- **Các thuộc tính chính:** `Id`, `OrderId`, `ProductType` (Xe/Phụ tùng/Dịch vụ), `ProductId` (ID của Xe hoặc Phụ tùng hoặc Dịch vụ tương ứng), `Quantity`, `Price`.

---

## 4. Quy tắc khởi tạo Dữ liệu mẫu (Seed Data Rules)
Khi khởi tạo cơ sở dữ liệu, AI cần tự động chèn dữ liệu mẫu tuân thủ đúng logic mối quan hệ:
1. Tạo 2 quốc gia: `Japan`, `Germany`.
2. Tạo các Brand tương ứng: `Honda` (Japan - Xe & Phụ tùng), `Toyota` (Japan - Xe), `Brembo` (Italy - Phụ tùng), `Akrapovic` (Slovenia - Đồ chơi/Pô).
3. Tạo sẵn các xe: `Honda Civic` (Auto, Xăng, Automatic, Số lượng: 5), `Yamaha Exciter` (Motorbike, Xăng, Clutch, Số lượng: NULL).
4. Tạo sẵn dịch vụ: `Rửa xe bọt tuyết` (Giá: 50.000), `Thay nhớt động cơ` (Giá: 150.000).

## 5. Quy Tắc Quản Lý Giá Và Dòng Tiền (Pricing & Financial Rules)

Để phục vụ bài toán mua đi bán lại xe cũ, bán phụ tùng mới và làm dịch vụ, hệ thống quy định nghiêm ngặt cách quản lý giá như sau:

### 5.1. Đối với Xe (Vehicles) - Mô hình Mua đi Bán lại Xe cũ
Do đặc thù xe cũ có giá biến động theo từng chiếc (tùy tình trạng xe lúc thu vào), bảng `Vehicles` cần tách rõ hai loại giá:
- **`PurchasePrice` (Giá Thu Vào):** Số tiền đại lý bỏ ra để mua chiếc xe đó từ chủ cũ.
- **`CurrentPrice` (Giá Bán Ra):** Giá niêm yết bán cho khách hàng mới (Luôn lớn hơn `PurchasePrice` để có lãi).
- *Công thức tính lợi nhuận gộp cho mỗi chiếc xe bán được:* `Profit = CurrentPrice - PurchasePrice`.

### 5.2. Đối với Phụ tùng (SpareParts) - Mô hình Bán mới từ Hãng
Vì tất cả phụ tùng đều là hàng mới nhập chính hãng theo lô, giá được quản lý nhất quán:
- **`CostPrice` (Giá Gốc/Giá Nhập):** Giá mua mới từ hãng khi nhập kho. Thuộc tính này dùng để tính giá trị hàng tồn kho.
- **`Price` (Giá Bán Lẻ):** Giá bán ra cho khách hàng khi họ mua lẻ hoặc thay thế khi làm dịch vụ.
- *Công thức tính lợi nhuận phụ tùng:* `Profit = Price - CostPrice`.

### 5.3. Đối với Dịch vụ (Services) - Mô hình Giá Cố Định
Dịch vụ không có chi phí nhập kho (vì bản chất là tiền công, chất xám và công cụ có sẵn của cửa hàng), nên chỉ quản lý **DUY NHẤT một loại giá**:
- **`BasePrice` (Giá Dịch Vụ):** Số tiền cố định khách phải trả cho dịch vụ đó (Ví dụ: Rửa xe 50.000đ, Công thay nhớt 30.000đ). Toàn bộ số tiền này được tính thẳng vào doanh thu thuần của cửa hàng.
nhưng với dịch vụ , phải phân biệt rõ ràng từng loại xe, 2 bánh và 4 bánh chênh lệch rõ ràng tuy cùng 1 loại dịch vụ, chi tiết như thế sẽ mục tiếp

Dịch vụ tại Auto Hub được chia làm 2 nhóm rõ rệt dựa trên loại xe và tính chất phức tạp:

### 5.3.1. Nhóm Dịch Vụ Cố Định (Fixed Price Services)
- Cùng một tên dịch vụ nhưng **bắt buộc phải tách làm 2 dòng riêng biệt** trong Database cho `Motorbike` và `Auto` với mức giá khác nhau.
- *Ví dụ dữ liệu mẫu:*
  - Dòng 1: `ServiceName` = N'Rửa xe bọt tuyết', `VehicleType` = 'Motorbike', `Price` = 50000, `RequiresQuote` = 0
  - Dòng 2: `ServiceName` = N'Rửa xe bọt tuyết', `VehicleType` = 'Auto', `Price` = 150000, `RequiresQuote` = 0

### 5.3.2. Nhóm Dịch Vụ Đặc Thù - Kích Hoạt Kênh Tư Vấn (Contact / Chatbox Services)
- Đối với các dịch vụ phức tạp không thể có giá cố định (Ví dụ: "Sửa chữa đại tu động cơ", "Độ đèn tăng sáng theo yêu cầu", "Đồng sơn phục hồi xe tai nạn"), giá cả sẽ phụ thuộc vào mức độ hư tổn của xe sau khi thợ kiểm tra.
- Lúc này, cột `Price` sẽ nhận giá trị `NULL` và cờ `RequiresQuote` = `1`.
- **Logic xử lý trên giao diện (UI/Console/Chatbox):** - Khi khách hàng chọn dịch vụ có `RequiresQuote == true`, hệ thống **không hiển thị giá tiền** mà sẽ hiển thị thông báo điều hướng hành vi:
    > *"Dịch vụ này cần khảo sát thực tế. Vui lòng bấm 1 để Chat với tổng đài viên, bấm 2 để lấy Hotline/Zalo liên hệ trực tiếp, hoặc để lại số điện thoại để nhận cuộc gọi tư vấn trong 5 phút."*