-- ============================================================
-- AutoHub – RunAll.sql
-- Chạy toàn bộ seed data theo đúng thứ tự phụ thuộc khoá ngoại.
-- Sử dụng: mở file này trong SSMS → Execute (F5)
--
-- Thứ tự:
--   01 SystemDictionaries  (không phụ thuộc)
--   02 Countries           (không phụ thuộc)
--   03 Brands              (→ Countries)
--   04 ProductCategories   (không phụ thuộc)
--   05 Vehicles            (→ Brands)
--   06 SpareParts          (→ Brands, ProductCategories, Vehicles)
--   07 Users               (không phụ thuộc)
--   08 Orders              (→ Users, Vehicles, SpareParts)
--   09 AddressData         (Province/District/Ward → SystemDictionaries)
-- ============================================================

:r "01_SystemDictionaries.sql"
:r "02_Countries.sql"
:r "03_Brands.sql"
:r "04_ProductCategories.sql"
:r "05_Vehicles.sql"
:r "06_SpareParts.sql"
:r "07_Users.sql"
:r "08_Orders.sql"
:r "09_AddressData.sql"

PRINT '=== AutoHub seed data loaded successfully. ===';
