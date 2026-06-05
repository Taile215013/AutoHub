"""
AutoHub – generate_address_sql.py
Đọc file Excel chính thống của nhà nước và sinh ra file SQL seed
cho bảng SystemDictionaries (Type = Province / District / Ward).

Yêu cầu: pip install openpyxl
Chạy:    python generate_address_sql.py
Output:  ../09_AddressData.sql
"""

import openpyxl
import re
import unicodedata
import os

EXCEL_PATH = r"D:\IT\C#\AutoHub\SQL Data\Source\Danh-muc-Phuong-xa_moi.xlsx"
OUTPUT_PATH = r"D:\IT\C#\AutoHub\SQL Data\09_AddressData.sql"

# ── Helpers ──────────────────────────────────────────────────────────────────

def slugify(text: str) -> str:
    """Chuyển tên tiếng Việt → slug ASCII không dấu, camelCase-like."""
    if not text:
        return ""
    # Bỏ dấu
    nfkd = unicodedata.normalize("NFKD", text)
    ascii_str = nfkd.encode("ascii", "ignore").decode("ascii")
    # Bỏ ký tự đặc biệt, giữ chữ/số/space
    cleaned = re.sub(r"[^a-zA-Z0-9\s]", "", ascii_str)
    # CamelCase từng từ, bỏ space
    slug = "".join(w.capitalize() for w in cleaned.split())
    return slug

def escape_sql(text: str) -> str:
    """Escape single quote cho SQL."""
    return text.replace("'", "''")

# ── Đọc Excel ────────────────────────────────────────────────────────────────

print(f"Đọc file: {EXCEL_PATH}")
wb = openpyxl.load_workbook(EXCEL_PATH, read_only=True, data_only=True)
ws = wb.active

# Cấu trúc theo phân tích:
# col[2] = Mã tỉnh (BNV)   – str "01".."34"
# col[3] = Tên tỉnh/TP mới
# col[5] = Mã Quận huyện   – int
# col[6] = Tên Quận huyện
# col[8] = Mã phường/xã    – int
# col[9] = Tên Phường/Xã mới

provinces: dict[str, str] = {}        # ma_tinh → ten_tinh
districts: dict[tuple, tuple] = {}    # (ma_tinh, ma_quan) → (ten_tinh_slug, ten_quan)
wards: list[tuple] = []               # (province_slug, district_slug, ward_slug, ten_phuong)

# slug cache tránh trùng
district_slug_cache: dict[str, dict[int, str]] = {}   # province_slug → {ma_quan: slug}

for row in ws.iter_rows(min_row=4, values_only=True):
    stt = row[1]
    if stt is None:
        continue

    ma_tinh  = str(row[2]).strip().zfill(2) if row[2] else ""
    ten_tinh = str(row[3]).strip() if row[3] else ""
    ma_quan  = int(row[5]) if row[5] else 0
    ten_quan = str(row[6]).strip() if row[6] else ""
    ma_phuong = int(row[8]) if row[8] else 0
    ten_phuong = str(row[9]).strip() if row[9] else ""

    if not ma_tinh or not ten_tinh:
        continue

    # Province
    if ma_tinh not in provinces:
        provinces[ma_tinh] = ten_tinh

    if not ten_quan or not ma_quan:
        continue

    # District slug: dùng tên quận slugify, nếu trùng thêm mã
    prov_slug = slugify(ten_tinh)
    if prov_slug not in district_slug_cache:
        district_slug_cache[prov_slug] = {}

    if ma_quan not in district_slug_cache[prov_slug]:
        dist_slug_base = slugify(ten_quan)
        # Kiểm tra slug đã tồn tại trong tỉnh này chưa
        used_slugs = set(district_slug_cache[prov_slug].values())
        dist_slug = dist_slug_base
        if dist_slug in used_slugs:
            dist_slug = f"{dist_slug_base}{ma_quan}"
        district_slug_cache[prov_slug][ma_quan] = dist_slug

    dist_slug = district_slug_cache[prov_slug][ma_quan]

    if (ma_tinh, ma_quan) not in districts:
        districts[(ma_tinh, ma_quan)] = (prov_slug, dist_slug, ten_quan)

    if not ten_phuong or not ma_phuong:
        continue

    # Ward slug
    ward_slug_base = slugify(ten_phuong)
    ward_slug = f"{ward_slug_base}{ma_phuong}"  # thêm mã để đảm bảo unique

    wards.append((prov_slug, dist_slug, ward_slug, ten_phuong))

wb.close()

print(f"  Tỉnh/TP:    {len(provinces)}")
print(f"  Quận/huyện: {len(districts)}")
print(f"  Phường/xã:  {len(wards)}")

# ── Sinh SQL ─────────────────────────────────────────────────────────────────

BATCH_SIZE = 500   # Chia MERGE thành nhiều batch để SSMS không bị quá tải

def chunks(lst, n):
    for i in range(0, len(lst), n):
        yield lst[i:i + n]

lines = []
lines.append("-- ============================================================")
lines.append("-- AutoHub – Seed: Address Data (Dữ liệu hành chính Việt Nam)")
lines.append("-- Sinh tự động từ file Excel chính thống của Nhà nước.")
lines.append(f"-- Tỉnh/TP: {len(provinces)}  |  Quận/Huyện: {len(districts)}  |  Phường/Xã: {len(wards)}")
lines.append("-- An toàn chạy lại nhiều lần (MERGE / upsert).")
lines.append("-- ============================================================")
lines.append("SET QUOTED_IDENTIFIER ON;")
lines.append("SET ANSI_NULLS ON;")
lines.append("GO")
lines.append("")

# ── PHẦN 1: TỈNH / THÀNH PHỐ ────────────────────────────────────────────────
lines.append("-- ============================================================")
lines.append(f"-- PHẦN 1: TỈNH / THÀNH PHỐ ({len(provinces)} đơn vị)")
lines.append("-- ============================================================")

prov_list = sorted(provinces.items())  # sort theo mã tỉnh

for batch in chunks(prov_list, BATCH_SIZE):
    lines.append(";WITH src(Type, Code, Value) AS (")
    lines.append("    SELECT N'Province', Code, Value FROM (VALUES")
    for i, (ma_tinh, ten_tinh) in enumerate(batch):
        prov_slug = slugify(ten_tinh)
        comma = "," if i < len(batch) - 1 else ""
        lines.append(f"        (N'{prov_slug}', N'{escape_sql(ten_tinh)}'){comma}")
    lines.append("    ) AS t(Code, Value)")
    lines.append(")")
    lines.append("MERGE SystemDictionaries AS tgt")
    lines.append("USING src ON tgt.Type = src.Type AND tgt.Code = src.Code")
    lines.append("WHEN NOT MATCHED BY TARGET THEN")
    lines.append("    INSERT (Type, Code, Value, CreatedAt, IsDeleted)")
    lines.append("    VALUES (src.Type, src.Code, src.Value, GETUTCDATE(), 0)")
    lines.append("WHEN MATCHED AND tgt.Value <> src.Value THEN")
    lines.append("    UPDATE SET tgt.Value = src.Value, tgt.UpdatedAt = GETUTCDATE();")
    lines.append("GO")
    lines.append("")

lines.append(f"PRINT 'Province seeded: {len(provinces)} records.';")
lines.append("GO")
lines.append("")

# ── PHẦN 2: QUẬN / HUYỆN ────────────────────────────────────────────────────
lines.append("-- ============================================================")
lines.append(f"-- PHẦN 2: QUẬN / HUYỆN ({len(districts)} đơn vị)")
lines.append("-- ============================================================")

dist_list = sorted(districts.items())  # sort theo (ma_tinh, ma_quan)

for batch in chunks(dist_list, BATCH_SIZE):
    lines.append(";WITH src(Type, Code, Value) AS (")
    lines.append("    SELECT N'District', Code, Value FROM (VALUES")
    for i, ((ma_tinh, ma_quan), (prov_slug, dist_slug, ten_quan)) in enumerate(batch):
        code = f"{prov_slug}|{dist_slug}"
        comma = "," if i < len(batch) - 1 else ""
        lines.append(f"        (N'{code}', N'{escape_sql(ten_quan)}'){comma}")
    lines.append("    ) AS t(Code, Value)")
    lines.append(")")
    lines.append("MERGE SystemDictionaries AS tgt")
    lines.append("USING src ON tgt.Type = src.Type AND tgt.Code = src.Code")
    lines.append("WHEN NOT MATCHED BY TARGET THEN")
    lines.append("    INSERT (Type, Code, Value, CreatedAt, IsDeleted)")
    lines.append("    VALUES (src.Type, src.Code, src.Value, GETUTCDATE(), 0)")
    lines.append("WHEN MATCHED AND tgt.Value <> src.Value THEN")
    lines.append("    UPDATE SET tgt.Value = src.Value, tgt.UpdatedAt = GETUTCDATE();")
    lines.append("GO")
    lines.append("")

lines.append(f"PRINT 'District seeded: {len(districts)} records.';")
lines.append("GO")
lines.append("")

# ── PHẦN 3: PHƯỜNG / XÃ ─────────────────────────────────────────────────────
lines.append("-- ============================================================")
lines.append(f"-- PHẦN 3: PHƯỜNG / XÃ ({len(wards)} đơn vị)")
lines.append("-- ============================================================")

for batch in chunks(wards, BATCH_SIZE):
    lines.append(";WITH src(Type, Code, Value) AS (")
    lines.append("    SELECT N'Ward', Code, Value FROM (VALUES")
    for i, (prov_slug, dist_slug, ward_slug, ten_phuong) in enumerate(batch):
        code = f"{prov_slug}|{dist_slug}|{ward_slug}"
        comma = "," if i < len(batch) - 1 else ""
        lines.append(f"        (N'{code}', N'{escape_sql(ten_phuong)}'){comma}")
    lines.append("    ) AS t(Code, Value)")
    lines.append(")")
    lines.append("MERGE SystemDictionaries AS tgt")
    lines.append("USING src ON tgt.Type = src.Type AND tgt.Code = src.Code")
    lines.append("WHEN NOT MATCHED BY TARGET THEN")
    lines.append("    INSERT (Type, Code, Value, CreatedAt, IsDeleted)")
    lines.append("    VALUES (src.Type, src.Code, src.Value, GETUTCDATE(), 0)")
    lines.append("WHEN MATCHED AND tgt.Value <> src.Value THEN")
    lines.append("    UPDATE SET tgt.Value = src.Value, tgt.UpdatedAt = GETUTCDATE();")
    lines.append("GO")
    lines.append("")

lines.append(f"PRINT 'Ward seeded: {len(wards)} records.';")
lines.append("GO")
lines.append("")
lines.append("PRINT '=== Address data seeded successfully. ===';")

# ── Ghi file ─────────────────────────────────────────────────────────────────
with open(OUTPUT_PATH, "w", encoding="utf-8-sig") as f:
    f.write("\n".join(lines))

size_kb = os.path.getsize(OUTPUT_PATH) // 1024
print(f"\nĐã tạo: {OUTPUT_PATH}")
print(f"Kích thước: {size_kb} KB")
print("Xong!")
