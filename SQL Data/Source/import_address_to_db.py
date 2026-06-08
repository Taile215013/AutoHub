"""
AutoHub – import_address_to_db.py
Đọc file SQL gốc Danh-muc-Phuong-xa_moi.sql và nạp thẳng vào
3 bảng: dbo.Provinces, dbo.Districts, dbo.Wards

Yêu cầu: pip install pyodbc
Kết nối: SQL Server cục bộ, trusted connection (Windows Auth)
"""

import re
import pyodbc
from datetime import datetime, timezone

SQL_FILE   = r"D:\IT\C#\AutoHub\SQL Data\Source\Danh-muc-Phuong-xa_moi.sql"
CONN_STR   = (
    "DRIVER={ODBC Driver 17 for SQL Server};"
    "SERVER=.;"
    "DATABASE=AutoHubDb;"
    "Trusted_Connection=yes;"
    "TrustServerCertificate=yes;"
)

# ── Đọc + parse file SQL gốc ─────────────────────────────────────────────────
print(f"Đọc file: {SQL_FILE}")

# Pattern: VALUES (NULL, 'STT', ...) → bỏ qua header
# Data row: VALUES (NULL, '1', '01', 'Thành phố Hà Nội', '101', '10105', 'Quận Hoàn Kiếm', '1', '10105001', 'Phường Hoàn Kiếm', ...)
PATTERN = re.compile(r"VALUES\s*\(([^)]+)\)", re.IGNORECASE)

def parse_values(raw: str) -> list[str | None]:
    """Parse chuỗi VALUES thành list Python, xử lý NULL và string có dấu phẩy."""
    items = []
    # Tách theo dấu phẩy không nằm trong quotes
    token = ""
    in_quote = False
    for ch in raw:
        if ch == "'" and not in_quote:
            in_quote = True
            token += ch
        elif ch == "'" and in_quote:
            in_quote = False
            token += ch
        elif ch == "," and not in_quote:
            items.append(token.strip())
            token = ""
        else:
            token += ch
    if token.strip():
        items.append(token.strip())

    result = []
    for item in items:
        s = item.strip()
        if s.upper() == "NULL":
            result.append(None)
        elif s.startswith("'") and s.endswith("'"):
            result.append(s[1:-1].replace("''", "'"))
        else:
            result.append(s)
    return result

provinces: dict[str, str] = {}   # code → name
districts: dict[str, tuple] = {} # code → (name, province_code)
wards: list[tuple] = []          # (code, name, district_code)

with open(SQL_FILE, encoding="utf-8") as f:
    for line in f:
        m = PATTERN.search(line)
        if not m:
            continue
        vals = parse_values(m.group(1))
        # cols: [0]=null, [1]=stt, [2]=mã tỉnh BNV, [3]=tên tỉnh,
        #        [4]=mã tỉnh TMS, [5]=mã quận TMS, [6]=tên quận,
        #        [7]=số tự tăng, [8]=mã phường mới, [9]=tên phường
        if len(vals) < 10:
            continue
        stt        = vals[1]
        ma_tinh    = str(vals[2]).strip().zfill(2) if vals[2] else ""
        ten_tinh   = str(vals[3]).strip() if vals[3] else ""
        ma_quan    = str(vals[5]).strip() if vals[5] else ""
        ten_quan   = str(vals[6]).strip() if vals[6] else ""
        ma_phuong  = str(vals[8]).strip() if vals[8] else ""
        ten_phuong = str(vals[9]).strip() if vals[9] else ""

        # Bỏ qua dòng header (stt = 'STT')
        if stt == "STT" or not ma_tinh.isdigit():
            continue

        if ma_tinh and ten_tinh and ma_tinh not in provinces:
            provinces[ma_tinh] = ten_tinh

        if ma_quan and ten_quan and ma_quan not in districts:
            districts[ma_quan] = (ten_quan, ma_tinh)

        if ma_phuong and ten_phuong:
            wards.append((ma_phuong, ten_phuong, ma_quan))

print(f"  Parse xong — Tỉnh: {len(provinces)}, Quận: {len(districts)}, Phường: {len(wards)}")

# ── Kết nối DB ───────────────────────────────────────────────────────────────
print("\nKết nối SQL Server...")
try:
    conn = pyodbc.connect(CONN_STR)
except Exception:
    # Thử ODBC Driver 18 nếu 17 không có
    CONN_STR2 = CONN_STR.replace("ODBC Driver 17", "ODBC Driver 18")
    conn = pyodbc.connect(CONN_STR2)

cursor = conn.cursor()
now = datetime.now(timezone.utc)

# ── Xóa dữ liệu cũ nếu có ───────────────────────────────────────────────────
print("Xóa dữ liệu cũ (nếu có)...")
cursor.execute("DELETE FROM dbo.Wards")
cursor.execute("DELETE FROM dbo.Districts")
cursor.execute("DELETE FROM dbo.Provinces")
conn.commit()

# ── Reset IDENTITY ──────────────────────────────────────────────────────────
cursor.execute("DBCC CHECKIDENT ('Wards',     RESEED, 0)")
cursor.execute("DBCC CHECKIDENT ('Districts', RESEED, 0)")
cursor.execute("DBCC CHECKIDENT ('Provinces', RESEED, 0)")
conn.commit()

# ── Nạp Provinces ────────────────────────────────────────────────────────────
print(f"Nạp {len(provinces)} tỉnh/thành phố...")
prov_data = [
    (code, name, now, now, False)
    for code, name in sorted(provinces.items())
]
cursor.executemany(
    "INSERT INTO dbo.Provinces (Code, Name, CreatedAt, UpdatedAt, IsDeleted) VALUES (?,?,?,?,?)",
    prov_data
)
conn.commit()
print(f"  ✓ Provinces: {cursor.rowcount} rows (cuối batch)")

# ── Nạp Districts ────────────────────────────────────────────────────────────
print(f"Nạp {len(districts)} quận/huyện...")
dist_data = [
    (code, name, prov_code, now, now, False)
    for code, (name, prov_code) in sorted(districts.items())
]
cursor.executemany(
    "INSERT INTO dbo.Districts (Code, Name, ProvinceCode, CreatedAt, UpdatedAt, IsDeleted) VALUES (?,?,?,?,?,?)",
    dist_data
)
conn.commit()

# ── Nạp Wards theo batch ─────────────────────────────────────────────────────
print(f"Nạp {len(wards)} phường/xã (batch 500)...")
ward_data = [
    (code, name, dist_code, now, now, False)
    for code, name, dist_code in wards
]

BATCH = 500
for i in range(0, len(ward_data), BATCH):
    batch = ward_data[i:i+BATCH]
    cursor.executemany(
        "INSERT INTO dbo.Wards (Code, Name, DistrictCode, CreatedAt, UpdatedAt, IsDeleted) VALUES (?,?,?,?,?,?)",
        batch
    )
    conn.commit()
    print(f"  batch {i//BATCH + 1}: {i+len(batch)}/{len(ward_data)}")

# ── Verify ───────────────────────────────────────────────────────────────────
cursor.execute("""
    SELECT 'Provinces' tbl, COUNT(*) n FROM dbo.Provinces WHERE IsDeleted=0
    UNION ALL
    SELECT 'Districts', COUNT(*) FROM dbo.Districts WHERE IsDeleted=0
    UNION ALL
    SELECT 'Wards',     COUNT(*) FROM dbo.Wards     WHERE IsDeleted=0
""")
print("\n=== Kết quả ===")
for row in cursor.fetchall():
    print(f"  {row[0]:12s}: {row[1]:,} bản ghi")

conn.close()
print("\nXong!")
