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
│
├── Source/
│   ├── Danh-muc-Phuong-xa_moi.xlsx   ← file Excel gốc Nhà nước (nguồn)
│   ├── Danh-muc-Phuong-xa_moi.sql    ← file SQL gốc export từ Excel
│   └── import_address_to_db.py       ← script nạp địa chỉ vào DB (chạy 1 lần)
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

## Dữ liệu địa chỉ hành chính

Lưu trong **3 bảng riêng** (không dùng SystemDictionaries), nạp từ file SQL gốc của Nhà nước:

| Bảng | FK | Mã | Ví dụ |
|------|----|----|-------|
| `dbo.Provinces` | — | `Code` CHAR(2) BNV | `"01"` = Thành phố Hà Nội |
| `dbo.Districts` | `ProvinceCode` | `Code` CHAR(5) TMS | `"10105"` = Quận Hoàn Kiếm |
| `dbo.Wards` | `DistrictCode` | `Code` CHAR(8) mới | `"10105001"` = Phường Hoàn Kiếm |

**Số lượng:** 34 tỉnh / 696 quận|huyện / 3.321 phường|xã — toàn bộ danh mục chính thức.

**Nạp lần đầu hoặc tái nạp:**
```bash
python "SQL Data\Source\import_address_to_db.py"
```

**Khi có file Excel Nhà nước mới:** thay `Source/Danh-muc-Phuong-xa_moi.sql` rồi chạy lại script trên.

**Query mẫu:**
```sql
-- Tất cả tỉnh/TP
SELECT Code, Name FROM dbo.Provinces WHERE IsDeleted=0 ORDER BY Code;

-- Quận/huyện của TP.HCM (mã "79")
SELECT Code, Name FROM dbo.Districts WHERE ProvinceCode='79' AND IsDeleted=0 ORDER BY Name;

-- Phường/xã của Quận 1 TP.HCM (mã "76001")
SELECT Code, Name FROM dbo.Wards WHERE DistrictCode='76001' AND IsDeleted=0 ORDER BY Name;
```

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
