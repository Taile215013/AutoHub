# Auto Hub Project - Web Architecture & Technical Requirements

## 1. Phong Cách Thiết Kế & Trải Nghiệm Người Dùng (UI/UX Direction)
Dự án Auto Hub định hướng theo phân khúc cao cấp, kết hợp hài hòa giữa sự trẻ trung năng động của kỷ nguyên số và sự sang trọng, tinh tế, đẳng cấp của các thương hiệu siêu xe lớn (Porsche, Mercedes-Benz, BMW, Aston Martin, ferrari).

### 1.1. Nguyên Lý Thiết Kế UI (Design Language)
- **Màu sắc chủ đạo (Color Palette):**
  - **Giai đoạn Hiện tại (Mặc định - Light Mode Cao Cấp):** - *Nền chính (Background):* Trắng kem lạnh (`#F8F9FA` hoặc `#F5F5F7` - tone màu nền đặc trưng của Apple và các hãng xe sang để tôn vinh hình ảnh sản phẩm).
    - *Chữ & Đường viền (Text & Borders):* Đen bóng Piano (`#111111`) hoặc Xám niken đậm (`#333333`) tạo độ tương phản cao, sắc nét và tinh tế.
    - *Màu điểm nhấn (Accent Color):* Vàng Gold Champagne nhạt, Đỏ Carmine Ferrari hoặc Xanh Neon Mint bão hòa thấp để làm nổi bật các nút bấm (Buttons) hoặc thẻ Hạng thành viên.
  - 
  /* Mặc định là Light Mode */
:root {
  --background-color: #F8F9FA;
  --text-color: #111111;
  --accent-color: #FF2828; /* Đỏ Ferrari */
  --button-color: blue,  Xanh Neon Mint, Đỏ, Vàng; /* Đen bóng Piano */
}


- **Bố cục (Layout):** Tối giản (Minimalism), tối ưu hóa khoảng trắng (Whitespace) để tôn vinh hình ảnh sản phẩm xe. Sử dụng các đường bo góc vuông vức hoặc bo nhẹ (Border-radius: 4px - 8px) mang lại cảm giác chắc chắn, cơ khí và cao cấp.
- **Hiệu ứng & Chuyển động (Animations & Transitions):**
  - Chuyển động phải mượt mà (Smooth scrolling, Fade-in, Slide-up nhẹ nhàng), sử dụng hiệu ứng `transition: all 0.3s ease-in-out`.
  - Hiệu ứng Hover vào thẻ sản phẩm: Xe hơi nhô lên hoặc phóng to nhẹ (Scale 1.02) kèm đổ bóng mờ hiệu ứng kính (Glassmorphism).

---

## 2. Đặc Tả Tính Năng Front-End (FE Requirements)

### 2.1. Trang Chủ & Hệ Thống Khách Hàng (HomePage & User Space)
- **Hero Section:** Banner tràn màn hình (Full-bleed) hiển thị video hoặc hình ảnh siêu xe chất lượng cao góc cạnh kèm slogan tinh tế.
- **Giao diện Tài khoản Cá nhân (User Dashboard):** - Thiết kế dạng thẻ Tab sang trọng bao gồm: Thông tin cá nhân, Lịch sử mua xe, Lịch sử bảo dưỡng/Dịch vụ, Hạng thành viên (`Bronze`, `Silver`, `Gold`, `Diamond`).
  - Thanh tiến trình nâng hạng thành viên (Loyalty Progress Bar) thiết kế dạng vi mạch tối giản.
- **Hệ thống Bộ lọc Thông minh (Smart Filter Bar):** - Bộ lọc động cho phép tìm kiếm theo: Loại xe (2 bánh/4 bánh), Dáng xe (Sedan/SUV/Scooter), Loại động cơ (Xăng/Điện/Dung tích).
  - **Địa chính 2026:** Form đăng ký/Cập nhật địa chỉ tích hợp Dropdown cascade. Khi chọn thành phố -> Hiện quận huyện mới theo Nghị định 2026 (Ví dụ: Gò Vấp) -> Hiện danh sách phường mới (Ví dụ: An Hội Tây).
- **Kênh Tương Tác Linh Hoạt:**
  - Đối với các dịch vụ/sản phẩm giá cố định: Hiển thị nút "Đặt lịch ngay".
  - Đối với xe cũ (Cần thương lượng) hoặc Dịch vụ đặc thù (`RequiresQuote = 1`): Ẩn giá, hiển thị nút "Liên hệ tư vấn" kích hoạt Popup Chatbox giả lập hoặc hiển thị Hotline/Zalo trực tiếp.

### 2.2. Giao Diện Quản Trị (Admin Pages)
- **Bố cục tổng thể:** Chia làm 2 phần cố định.
  - **Bên trái (Sidebar Nav):** Thanh menu điều hướng cố định (Sticky Sidebar) phong cách tối giản chứa các mục: Tổng quan (Dashboard), Quản lý Xe (Vehicles), Quản lý Phụ tùng (Spare Parts), Quản lý Dịch vụ (Services), Quản lý Hóa đơn (Orders).
  - **Bên phải (Main Content):** Không gian hiển thị bảng dữ liệu (Data Table) và các form thêm/sửa sản phẩm.
- **Tính năng Quản lý Sản phẩm (CRUD Operations):**
  - Bảng hiển thị danh sách sản phẩm sạch sẽ, có phân trang, có bộ lọc nhanh.
  - Khi thêm xe cũ: Cho phép nhập cả `PurchasePrice` (Giá thu vào) và `CurrentPrice` (Giá bán ra).
  - Khi thêm màu sắc cho xe: Giao diện cho phép chọn thêm nhiều hàng (Row) màu sắc khác nhau (Mỗi dòng 1 màu có dấu: Đỏ, Xanh...) thay vì nhập gom chung.

---

## 3. Đặc Tả Hệ Thống Back-End (BE & Database Requirements)

### 3.1. Kiến Trúc Mã Nguồn (C# & Dapper)
- **Kiến trúc:** Triển khai theo mô hình **Repository Pattern**. Tách biệt hoàn toàn logic SQL ra khỏi tầng hiển thị giao diện.
- **Thư viện kết nối:** Sử dụng **Dapper** để tối ưu hóa hiệu năng, viết truy vấn SQL thuần có tham số (`Parameterized Queries`) để chống lỗ hổng SQL Injection.
- **Logic Nghiệp vụ Tích hợp:**
  - Hàm cập nhật tài khoản: Bắt buộc xác thực lại mật khẩu cũ trước, sau đó quét DB xem SĐT/Email mới có bị trùng với tài khoản khác không (`Unique Check`) rồi mới cho phép `UPDATE`.
  - Hàm tính tiền hóa đơn: Tự động kiểm tra `RankLevel` của `User` để áp mã giảm giá theo hạng thành viên (Silver -2%, Gold -5%, Diamond -10%).

---

## 4. Hướng Dẫn Thiết Lập Cơ Sở Dữ Liệu SQL Server (Database Setup Guide)

Để hệ thống kết nối suôn sẻ và không bị lỗi font tiếng Việt khi lưu trữ chuỗi có dấu (Ví dụ: "Đỏ", "Xám Xi Măng", "An Hội Tây"), cấu hình SQL Server cần tuân thủ các bước thiết lập sau:

### 4.1. Thiết lập Collation (Bảng mã tiếng Việt)
Khi tạo mới Database cho dự án Auto Hub, bắt buộc phải chọn Collation hỗ trợ tốt nhất cho tiếng Việt và không phân biệt chữ hoa chữ thường:
- **Cấu hình khuyên dùng:** `SQL_Latin1_General_CP1_CI_AS` hoặc `Vietnamese_CI_AS`.
- *Lệnh SQL khởi tạo chuẩn:*
  ```sql
  CREATE DATABASE AutoHubDb COLLATE SQL_Latin1_General_CP1_CI_AS;
  GO