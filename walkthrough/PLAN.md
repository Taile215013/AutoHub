# PLAN

File này dùng để quản lý danh sách yêu cầu, chức năng chưa làm và thứ tự triển khai của dự án AutoHub.

Mục tiêu chính: làm từng module hoàn chỉnh, logic dễ hiểu, phù hợp báo cáo cuối kỳ và không overengineering.

## Quy tắc triển khai

1. Mỗi tính năng đi theo luồng: `database` -> `model` -> `service/repository` -> `controller` -> `view`.
2. Làm xong 1 module hoàn chỉnh rồi mới sang module tiếp theo.
3. Ưu tiên tính năng lõi trước, tính năng mạng xã hội / cộng đồng làm sau.
4. Giữ code đơn giản, dễ giải thích, tránh kiến trúc quá phức tạp.
5. Mỗi module nên có CRUD, validation cơ bản, giao diện admin và giao diện người dùng nếu cần.
6. Phân biệt rõ dữ liệu nền tảng của hệ thống và dữ liệu phát sinh từ người dùng.
7. Dữ liệu nền tảng không nên xóa cứng khỏi database nếu đang được module khác sử dụng.

## Mức ưu tiên

- `P0`: Bắt buộc nên làm trước để dự án chạy được nghiệp vụ chính.
- `P1`: Nên làm để dự án đầy đủ và dễ báo cáo.
- `P2`: Làm sau khi các phần chính đã ổn.
- `P3`: Tính năng mở rộng, có thể để giai đoạn sau.

## Trạng thái

- `TODO`: Chưa làm.
- `DOING`: Đang làm.
- `DONE`: Đã làm xong.
- `LATER`: Để sau.

## Roadmap đề xuất

1. Hoàn thiện dữ liệu xe và phân loại xe.
2. Hoàn thiện upload nhiều ảnh cho xe.
3. Hoàn thiện module thu mua / thẩm định xe cũ.
4. Hoàn thiện dịch vụ và booking.
5. Hoàn thiện phụ tùng, category và bộ lọc phụ tùng theo xe.
6. Hoàn thiện trang người dùng sau đăng nhập.
7. Làm tìm kiếm, bộ lọc mạnh, yêu thích, so sánh xe.
8. Làm cộng đồng, bài viết, chat tư vấn và đánh giá dịch vụ.

## Nguyên tắc lưu dữ liệu

Trong AutoHub cần tách dữ liệu thành 2 nhóm:

| Nhóm dữ liệu | Ví dụ | Cách xử lý khi xóa |
| --- | --- | --- |
| Dữ liệu nền tảng / master data | Brand, model, dòng xe, đời xe, loại xe, category, classification | Không xóa cứng nếu đang có dữ liệu liên quan. Nên dùng soft delete hoặc trạng thái `Inactive`. |
| Dữ liệu phát sinh từ người dùng | Ảnh xe người dùng upload, ODO xe cũ, mô tả tình trạng xe, bài viết cộng đồng, ảnh chuyến đi | Có thể xóa, ẩn hoặc lưu lịch sử tùy nghiệp vụ. |

Ví dụ: người dùng nhập xe `Airblade` của `Honda`. Sau đó dữ liệu brand/model đã có trong database thì admin mới có thể gắn phụ tùng cho đúng xe đó. Nếu xóa cứng `Honda` hoặc `Airblade`, các dữ liệu liên quan như phụ tùng, bộ lọc, xe đã đăng, lịch sử thẩm định có thể bị mất liên kết.

Quy tắc đề xuất:

1. `Brands`, `VehicleNames`, `VehicleModels`, `VehicleTypes`, `Categories`, `Classifications` nên là dữ liệu nền tảng.
2. Khi admin muốn xóa dữ liệu nền tảng, hệ thống nên kiểm tra dữ liệu liên quan trước.
3. Nếu dữ liệu đang được sử dụng, chỉ cho phép chuyển sang `Inactive` hoặc `IsDeleted = true`.
4. Giao diện người dùng không hiển thị dữ liệu đã bị ẩn, nhưng database vẫn giữ để bảo toàn lịch sử và quan hệ.
5. Dữ liệu riêng của hồ sơ xe người dùng như ảnh, ODO, mô tả tình trạng có thể được xóa hoặc ẩn theo yêu cầu.
6. Các bảng quan hệ nên dùng khóa ngoại để tránh dữ liệu bị rời rạc.

Nguyên tắc tên xe:

1. Tên xe gốc nên chỉ tồn tại một lần trong database, ví dụ `Airblade`.
2. Khi nhập xe mới, hệ thống nên chọn từ dữ liệu gốc thay vì nhập lại tên tự do.
3. Phân khối / phiên bản như `125`, `150`, `160` cũng nên là master data vì phụ tùng có thể khác nhau theo từng phiên bản.
4. Năm đời xe cũng cần được lưu rõ vì cùng một dòng xe nhưng khác năm có thể dùng phụ tùng khác.
5. Ví dụ: `Airblade 150` năm `2022` có thể dùng phụ tùng khác hoàn toàn `Airblade 160` năm `2023`.
6. Giai đoạn đầu cần làm chắc phần tên xe gốc trước để tránh trùng dữ liệu như `Airblade`, `Air Blade`, `airblade`.
7. Sau đó mở rộng sang bảng phiên bản / phân khối / năm đời xe để gắn phụ tùng chính xác hơn.

Cấu trúc master data đề xuất cho xe:

| Dữ liệu | Ví dụ | Mục đích |
| --- | --- | --- |
| Brand | Honda | Hãng xe. |
| VehicleName | Airblade | Tên xe gốc, chỉ lưu một lần. |
| VehicleVariant | 125, 150, 160 | Phiên bản / phân khối / biến thể. |
| ModelYear | 2022, 2023 | Năm đời xe hoặc năm sản xuất áp dụng. |
| VehicleGeneration | Gen 5, Gen 6 nếu cần | Đời xe / thế hệ xe, xử lý sau nếu dữ liệu đủ lớn. |

Nguyên tắc phụ tùng theo xe:

1. Phụ tùng không nên chỉ gắn với tên xe gốc nếu có khác biệt theo phiên bản.
2. Phụ tùng nên có thể gắn với `Brand` + `VehicleName` + `VehicleVariant` + `ModelYear`.
3. Nếu phụ tùng dùng chung nhiều phiên bản, tạo bảng liên kết nhiều-nhiều giữa phụ tùng và biến thể xe.
4. Giai đoạn đầu có thể làm đơn giản, nhưng database nên chuẩn bị hướng mở rộng để không phải sửa lại quá nhiều.

## Danh sách yêu cầu

| STT | Module | Yêu cầu | Ưu tiên | Trạng thái |
| --- | --- | --- | --- | --- |
| 0 | Classification | Hiện tại dự án mới phân loại ở mức cơ bản vì dữ liệu còn ít. Cần mở rộng phân loại cho xe, phụ tùng, dịch vụ. | P0 | TODO |
| 1 | Vehicle | Bổ sung dữ liệu hệ thống cho xe: giá bán, tình trạng, năm sản xuất, brand, model, màu, nhiên liệu, số km, chính chủ hay không, dòng xe, đời xe, ODO. | P0 | TODO |
| 2 | Used Vehicle Acquisition | Bổ sung dữ liệu thẩm định xe cũ: tình trạng máy, thân vỏ, nội thất, giấy tờ pháp lý, giá mong muốn, giá admin định giá. | P0 | TODO |
| 3 | Vehicle Images | Hiện tại chỉ upload 1 ảnh bìa. Cần hỗ trợ upload nhiều ảnh cho xe. | P0 | TODO |
| 4 | Vehicle Status | Xe có trạng thái còn bán, đã bán. Logic chi tiết sẽ xử lý sau. | P1 | TODO |
| 5 | Service | Dịch vụ có thể upload nhiều ảnh giống như xe hơi. | P1 | TODO |
| 6 | Spare Parts | Phụ tùng có thể upload ảnh. Cần trạng thái, năm sản xuất, dùng cho xe gì. | P0 | TODO |
| 7 | Spare Parts Status | Phụ tùng có trạng thái đang về hàng. | P1 | TODO |
| 8 | Category | Tạo category theo từng loại để phân loại xe, phụ tùng, đồ chơi, dịch vụ. | P0 | TODO |
| 9 | Recommendation | Khi người dùng chọn xe của họ hoặc xe muốn mua, web show phụ kiện, đồ chơi, phụ tùng liên quan theo classification. | P2 | TODO |
| 10 | User Page | Trang người dùng sau đăng nhập có giao diện giống mạng xã hội mức cơ bản, có sidebar nhiều chức năng. | P1 | TODO |
| 11 | User Profile | Hiện tại đã có thông tin liên hệ và có thể thay đổi. Sau này mở rộng thêm chức năng khác trong sidebar. | P1 | TODO |
| 12 | User Address | Một user có thể nhập nhiều địa chỉ giao hàng, nhưng chỉ có 1 địa chỉ được set mặc định là nhà. | P1 | TODO |
| 13 | Community | Xây dựng trang cộng đồng giống mạng xã hội mức cơ bản. Người dùng có thể đăng bài viết, hình ảnh về xe, chuyến đi. | P2 | TODO |
| 14 | Chat | Tin nhắn / chat tư vấn. | P3 | TODO |
| 15 | Favorite | Yêu thích / lưu xe. | P1 | TODO |
| 16 | Compare | So sánh xe. | P2 | TODO |
| 17 | Search & Filter | Tìm kiếm và bộ lọc mạnh. | P1 | TODO |
| 18 | Review | Đánh giá dịch vụ. | P2 | TODO |
| 19 | Data Retention | Tách dữ liệu nền tảng và dữ liệu người dùng. Dữ liệu nền tảng như brand, model, category không xóa cứng nếu đang được sử dụng. | P0 | TODO |
| 20 | Vehicle Name Master Data | Tên xe gốc như Airblade chỉ lưu một lần trong database. Khi nhập xe mới sẽ chọn từ dữ liệu gốc để tránh trùng tên. | P0 | TODO |
| 21 | Vehicle Variant Master Data | Phân khối / phiên bản như 125, 150, 160 là master data vì phụ tùng có thể khác nhau theo từng biến thể. | P0 | TODO |
| 22 | Vehicle-Part Compatibility | Phụ tùng cần gắn đúng xe, phiên bản và năm đời xe. Ví dụ AB150 2022 có thể khác AB160 2023. | P0 | TODO |

## Module thu mua / thẩm định xe cũ

Đây là mô hình mua đi bán lại, không phải người dùng tự bán trực tiếp cho người dùng khác.

Luồng nghiệp vụ đề xuất:

1. Người dùng đăng thông tin xe cũ thật chi tiết.
2. Admin xem hồ sơ xe và liên hệ người dùng.
3. Admin đặt lịch kiểm tra xe nếu hồ sơ hợp lệ.
4. Admin kiểm tra thực tế: giấy tờ, tình trạng máy, thân vỏ, nội thất, ODO, lịch sử xe nếu có.
5. Admin định giá hoặc thương lượng với người dùng.
6. Nếu đạt yêu cầu, admin nhận xe / mua xe / ký gửi xe.
7. Admin đăng xe lên page để bán lại.
8. Xe đăng bán là xe đã được kiểm duyệt, giúp hệ thống uy tín hơn rao vặt thông thường.

Trạng thái đề xuất:

| Status | Ý nghĩa |
| --- | --- |
| PendingReview | Người dùng vừa gửi yêu cầu, admin chưa xem. |
| Contacted | Admin đã liên hệ người dùng. |
| InspectionScheduled | Đã đặt lịch kiểm tra xe. |
| Inspected | Đã kiểm tra xe thực tế. |
| Rejected | Xe không đạt yêu cầu hoặc giấy tờ không hợp lệ. |
| Approved | Xe đạt yêu cầu, có thể tiến hành mua / ký gửi. |
| Purchased | AutoHub đã nhận xe / mua xe. |
| ListedForSale | Admin đã đăng xe lên page để bán lại. |
| Sold | Xe đã bán. |

## Thứ tự làm chi tiết

### Giai đoạn 1: Nền tảng dữ liệu

| STT | Việc cần làm | Trạng thái |
| --- | --- | --- |
| 1 | Kiểm tra lại database hiện tại có những bảng nào. | TODO |
| 2 | Chuẩn hóa bảng `Brands`, `Vehicles`, `Services`, `SpareParts`, `Categories`. | TODO |
| 3 | Thêm bảng ảnh riêng cho xe, dịch vụ, phụ tùng nếu cần. | TODO |
| 4 | Thêm enum / status cho xe và phụ tùng. | TODO |
| 5 | Tách dữ liệu nền tảng như brand, model, category khỏi dữ liệu phát sinh của người dùng. | TODO |
| 6 | Thêm cơ chế soft delete / inactive cho dữ liệu nền tảng đang được sử dụng. | TODO |
| 7 | Tạo dữ liệu gốc cho tên xe để tránh nhập trùng tên xe. | TODO |
| 8 | Tạo dữ liệu gốc cho phiên bản / phân khối xe. | TODO |
| 9 | Chuẩn bị quan hệ giữa phụ tùng và xe theo tên xe, phiên bản, năm đời xe. | TODO |

### Giai đoạn 2: Admin CRUD

| STT | Việc cần làm | Trạng thái |
| --- | --- | --- |
| 1 | Admin quản lý xe đầy đủ thông tin. | TODO |
| 2 | Admin quản lý ảnh xe nhiều cái. | TODO |
| 3 | Admin quản lý dịch vụ và ảnh dịch vụ. | TODO |
| 4 | Admin quản lý phụ tùng và ảnh phụ tùng. | TODO |
| 5 | Admin quản lý category / classification. | TODO |

### Giai đoạn 3: Public site

| STT | Việc cần làm | Trạng thái |
| --- | --- | --- |
| 1 | Trang danh sách xe có filter cơ bản. | TODO |
| 2 | Trang chi tiết xe hiển thị đầy đủ ảnh và thông tin. | TODO |
| 3 | Trang danh sách dịch vụ. | TODO |
| 4 | Trang chi tiết dịch vụ và đặt lịch. | TODO |
| 5 | Trang phụ tùng có filter theo xe / category. | TODO |

### Giai đoạn 4: User features

| STT | Việc cần làm | Trạng thái |
| --- | --- | --- |
| 1 | Trang user dashboard có sidebar. | TODO |
| 2 | User cập nhật thông tin cá nhân. | TODO |
| 3 | User quản lý nhiều địa chỉ giao hàng. | TODO |
| 4 | User lưu xe yêu thích. | TODO |
| 5 | User gửi yêu cầu thẩm định xe cũ. | TODO |

### Giai đoạn 5: Mở rộng

| STT | Việc cần làm | Trạng thái |
| --- | --- | --- |
| 1 | So sánh xe. | TODO |
| 2 | Trang cộng đồng và bài viết có hình ảnh. | TODO |
| 3 | Chat tư vấn. | TODO |
| 4 | Đánh giá dịch vụ. | TODO |
| 5 | Gợi ý phụ tùng / đồ chơi theo xe người dùng chọn. | TODO |
