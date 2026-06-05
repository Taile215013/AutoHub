(Xong. Tóm tắt những gì đã làm:

Script 
generate_address_sql.py
 — đọc trực tiếp file Excel nhà nước, tự động:

Slugify tên tỉnh/quận/phường thành code ASCII
Xử lý slug trùng (thêm mã vào cuối)
Chia MERGE thành batch 500 dòng để SSMS không bị quá tải
09_AddressData.sql (312 KB, 4210 dòng) — sinh từ Excel với:

34 tỉnh/thành phố
696 quận/huyện
3.321 phường/xã — toàn bộ danh mục chính thức
Khi Nhà nước cập nhật lại phân chia hành chính, chỉ cần thay file Excel mới vào Source/ rồi chạy lại python generate_address_sql.py là có SQL mới ngay.)
# AutoHub – SQL Data

Thư mục này chứa toàn bộ script SQL để khởi tạo và nạp dữ liệu cho database **AutoHubDB**.

---

## Cấu trúc thư mục

```
SQL Data/
├── README.md                 ← file này
│
├── RunAll.sql                ← chạy tất cả seed theo đúng thứ tự (dùng sqlcmd / SSMS)
├── ResetAll.sql              ← xóa sạch + reset IDENTITY (chỉ dùng môi trường DEV/TEST)
│
├── 01_SystemDictionaries.sql ← master data: loại xe, hộp số, màu, chức vụ, v.v.
├── 02_Countries.sql          ← 8 quốc gia
├── 03_Brands.sql             ← thương hiệu xe + phụ tùng (liên kết Countries)
├── 04_ProductCategories.sql  ← danh mục sản phẩm phụ tùng
├── 05_Vehicles.sql           ← dòng xe, phiên bản, năm model, xe tồn kho, màu sắc
├── 06_SpareParts.sql         ← phụ tùng + tương thích xe
├── 07_Users.sql              ← tài khoản người dùng mẫu
├── 08_Orders.sql             ← đơn hàng + chi tiết đơn hàng mẫu
├── 09_AddressData.sql        ← dữ liệu hành chính VN (tỉnh/quận/phường)
│
└── migrations/
    ├── migrate_v1.sql        ← thêm cột Username, DateOfBirth, ImageUrl
    └── migrate_v2.sql        ← xóa constraint unique thừa trên Email/PhoneNumber
```

---

## Hướng dẫn sử dụng

### Lần đầu thiết lập (fresh install)

1. Chạy EF Core migrations để tạo schema:
   ```bash
   dotnet ef database update
   ```

2. Nạp toàn bộ seed data bằng sqlcmd:
   ```bash
   sqlcmd -S localhost -d AutoHubDB -E -i "RunAll.sql"
   ```
   Hoặc mở `RunAll.sql` trong **SSMS** và nhấn **F5**.

### Reset dữ liệu (DEV/TEST only)

```bash
sqlcmd -S localhost -d AutoHubDB -E -i "ResetAll.sql"
sqlcmd -S localhost -d AutoHubDB -E -i "RunAll.sql"
```

> ⚠️ **Không chạy `ResetAll.sql` trên production.**

### Chạy từng file riêng lẻ

Mỗi file seed có thể chạy độc lập, miễn là các bảng phụ thuộc đã có dữ liệu (xem thứ tự phụ thuộc bên dưới). Tất cả script đều dùng `MERGE` hoặc `WHERE NOT EXISTS` nên **an toàn chạy lại nhiều lần** mà không bị lỗi duplicate.

---

## Thứ tự phụ thuộc

```
01_SystemDictionaries   (độc lập)
02_Countries            (độc lập)
03_Brands               → Countries
04_ProductCategories    (độc lập)
05_Vehicles             → Brands
                          (gồm: VehicleNames → VehicleVariants → VehicleModelYears
                                → Vehicles → VehicleColors)
06_SpareParts           → Brands, ProductCategories, VehicleNames/Variants/Years
07_Users                (độc lập)
08_Orders               → Users, Vehicles, SpareParts
09_AddressData          (độc lập – chèn vào SystemDictionaries)
```

---

## Dữ liệu địa chỉ (`09_AddressData.sql`)

Sinh tự động từ **file Excel chính thống của Nhà nước** bằng script `Source/generate_address_sql.py`.  
Để tái sinh (khi có file Excel mới): `python "SQL Data\Source\generate_address_sql.py"`

Dữ liệu được lưu vào bảng **SystemDictionaries** theo 3 `Type`:

| Type       | Code format                                       | Ví dụ Value              |
|------------|---------------------------------------------------|--------------------------|
| `Province` | `ProvinceSlug`                                    | `Thành phố Hà Nội`       |
| `District` | `ProvinceSlug\|DistrictSlug`                      | `Quận Hoàn Kiếm`         |
| `Ward`     | `ProvinceSlug\|DistrictSlug\|WardSlug{MaPhuong}`  | `Phường Hoàn Kiếm`       |

**Mức độ bao phủ (từ file Excel Nhà nước):**
- **Province**: 34 tỉnh/thành phố (theo phân chia hành chính hiện hành)
- **District**: 696 quận/huyện/thị xã/TP thuộc tỉnh
- **Ward**: 3.321 phường/xã/thị trấn — **toàn bộ** theo danh mục chính thức

**Cách query trong code:**
```sql
-- Lấy tất cả tỉnh/thành phố
SELECT Code, Value FROM SystemDictionaries
WHERE Type = 'Province' AND IsDeleted = 0
ORDER BY Value;

-- Lấy tất cả quận/huyện của Hà Nội
SELECT Code, Value FROM SystemDictionaries
WHERE Type = 'District' AND Code LIKE 'ThanhPhoHaNoi|%' AND IsDeleted = 0
ORDER BY Value;

-- Lấy tất cả phường của Quận Hoàn Kiếm
SELECT Code, Value FROM SystemDictionaries
WHERE Type = 'Ward' AND Code LIKE 'ThanhPhoHaNoi|QuanHoanKiem|%' AND IsDeleted = 0
ORDER BY Value;
```

> Slug tỉnh/quận dùng tên đầy đủ (kể cả "Thành phố", "Tỉnh", "Quận", "Huyện") để đúng với dữ liệu gốc. Ví dụ TP.HCM = `ThanhPhoHoChiMinh`, Quận 1 = `Quan1`.

---

## Tài khoản mẫu

| Username    | Email                    | Password | Rank |
|-------------|--------------------------|----------|------|
| `tainguyen` | tai.nguyen@autohub.vn    | `123456` | Gold |

> Password hash dùng SHA-256. Để thêm tài khoản mới, tính hash tại: https://emn178.github.io/online-tools/sha256.html

---

## Migrations

Các file trong `migrations/` là script SQL thủ công chạy **một lần duy nhất** để vá schema trước khi EF migrations được áp dụng. Nếu bạn dùng `dotnet ef database update` từ đầu trên DB mới, **không cần chạy** các file này.

| File              | Nội dung                                                   |
|-------------------|------------------------------------------------------------|
| `migrate_v1.sql`  | Thêm `Username`, `DateOfBirth`, `ImageUrl` vào bảng cũ    |
| `migrate_v2.sql`  | Xóa constraint unique thừa trên `Email` / `PhoneNumber`   |
