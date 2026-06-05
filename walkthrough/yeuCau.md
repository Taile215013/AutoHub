# AI Agent Instructions - Project: Auto Hub

## 1. Context & Objective (Ngữ cảnh & Mục tiêu)
- **Project Name:** Auto Hub (Hệ thống/Ứng dụng quản lý, vận hành hoặc dịch vụ ô tô).
- **Target:** Viết mã nguồn hoàn chỉnh, có tính thực tế cao và logic chặt chẽ để nộp báo cáo/luận văn. 
- **Audience:** Code sẽ được kiểm tra khắt khe bởi người hướng dẫn (Mentor/Supervisor).

---

## 2. Core Constraints (Các hạn chế cốt lõi - Bắt buộc)
- **No Comments:** TUYỆT ĐỐI KHÔNG viết comment giải thích trong code (vì người dùng sẽ tự báo cáo, giải trình trực tiếp).
- **Self-Documenting Code:** Code phải tự giải thích thông qua cấu trúc, cách đặt tên lớp (class) và hàm (method).

---

## 3. Architecture & Design Principles (Kiến trúc & Nguyên lý Thiết kế)
AI phải áp dụng nghiêm ngặt các tiêu chuẩn **Clean Code** và **SOLID Principles**:

### Single Responsibility Principle (SRP)
- Mỗi class, module, hoặc function chỉ thực hiện một nhiệm vụ duy nhất.
- Không viết các hàm "vạn năng" (God-functions).

### Open/Closed Principle (OCP)
- Thiết kế hệ thống dễ dàng mở rộng (extension) nhưng đóng với việc sửa đổi (modification).
- Ưu tiên sử dụng Polymorphism (Đa hình), Interface, hoặc Abstract Class thay vì lạm dụng `if-else` hoặc `switch-case` khi thêm tính năng mới cho Auto Hub (Ví dụ: Thêm loại xe mới, phương thức thanh toán mới).

### Liskov Substitution Principle (LSP)
- Các lớp con phải có thể thay thế lớp cha mà không làm thay đổi tính đúng đắn của chương trình.

### Interface Segregation Principle (ISP)
- Không ép buộc các class triển khai (implement) những interface chứa các hàm mà chúng không sử dụng. Tách nhỏ các interface nếu cần.

### Dependency Inversion Principle (DIP)
- Các module cấp cao không nên phụ thuộc vào các module cấp thấp. Cả hai nên phụ thuộc vào sự trừu tượng (Abstraction/Interface).
- Áp dụng Dependency Injection (DI) nếu ngôn ngữ yêu cầu.

---

## 4. Coding & Naming Standards (Quy chuẩn Lập trình & Đặt tên)
- **Readability & Maintainability:** Code phải cực kỳ dễ đọc, dễ hiểu, dễ bảo trì. Cấu trúc thư mục và file rõ ràng.
- **Naming Conventions:** - Tên biến, tên hàm, tên class phải mang tính gợi nhớ cao, rõ nghĩa (Clear and Descriptive).
  - Sử dụng đúng chuẩn quy ước của ngôn ngữ lập trình được chỉ định (ví dụ: `camelCase` cho Java/JS, `snake_case` cho Python, `PascalCase` cho C#...).
  - Không đặt tên biến viết tắt vô nghĩa (Ví dụ: Thay vì `d`, dùng `deliveryDate`; thay vì `c`, dùng `customer`).

---

## 5. Logic & Error Handling (Tư duy Logic & Xử lý lỗi)
Vì code chịu sự giám sát và chấm điểm chặt chẽ của người hướng dẫn, logic phải **cực kỳ chặt chẽ**:
- **Defensive Programming:** Luôn kiểm tra dữ liệu đầu vào (Validation) như check `null`, `undefined`, rỗng, hoặc sai định dạng trước khi xử lý logic.
- **Exception Handling:** Sử dụng các khối `try-catch` một cách tường minh. Không "nuốt" lỗi (catch nhưng để trống). Throw ra các Exception có ý nghĩa.
- **Edge Cases:** Xử lý triệt để các trường hợp biên (Ví dụ: Số lượng bằng 0, giá trị âm, dữ liệu trùng lặp).

---

## 6. Output Format Request (Yêu cầu Định dạng Đầu ra)
- Chỉ cung cấp mã nguồn (Pure Code blocks).
- Không giải thích dông dài trước hoặc sau khối code.
- Nếu có cấu trúc thư mục mới, hãy mô tả bằng sơ đồ cây thư mục (Tree structure).