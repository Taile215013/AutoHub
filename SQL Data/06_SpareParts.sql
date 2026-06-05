-- ============================================================
-- AutoHub – Seed: SpareParts + SparePartCompatibilities (Phụ tùng)
-- Yêu cầu: Brands (03), ProductCategories (04), Vehicles (05) đã có dữ liệu.
-- An toàn để chạy lại (bỏ qua nếu Id đã tồn tại).
-- ============================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ------------------------------------------------------------
-- 1. SpareParts
-- ------------------------------------------------------------
SET IDENTITY_INSERT SpareParts ON;

INSERT INTO SpareParts (Id, Name, BrandId, CategoryId, Category, Status, CostPrice, Price, StockQuantity, CreatedAt, IsDeleted)
SELECT v.Id, v.Name, v.BrandId, v.CategoryId, v.Category, v.Status, v.CostPrice, v.Price, v.StockQuantity, GETUTCDATE(), 0
FROM (VALUES
    -- Id  Name                                  BrandId  CatId  Category    Status      CostPrice    Price      Stock
    (1, N'Bugi Bosch Bạch Kim',               4, 1, N'Engine',   N'InStock',  120000.00,  250000.00, 100),
    (2, N'Má phanh trước Toyota Camry',        2, 2, N'Brake',    N'InStock',  850000.00, 1450000.00,  40),
    (3, N'Gạt mưa silicon VinFast VF8',        1, 3, N'Exterior', N'InStock',  300000.00,  550000.00,  50),
    (4, N'Lọc gió động cơ K&N',               5, 1, N'Engine',   N'InStock',  450000.00,  800000.00,  30),
    (5, N'Dầu động cơ cao cấp Liqui Moly 5W-30', 4, 1, N'Engine', N'InStock', 600000.00,  950000.00,  60),
    (6, N'Lọc dầu Astra Indonesia',           8, 1, N'Engine',   N'InStock',   90000.00,  180000.00, 120),
    (7, N'Khung sườn Thai Summit',             9, 3, N'Exterior', N'InStock', 2500000.00, 4200000.00,  15)
) AS v(Id, Name, BrandId, CategoryId, Category, Status, CostPrice, Price, StockQuantity)
WHERE NOT EXISTS (SELECT 1 FROM SpareParts WHERE Id = v.Id);

SET IDENTITY_INSERT SpareParts OFF;
GO

-- ------------------------------------------------------------
-- 2. SparePartCompatibilities (Tương thích xe)
-- ------------------------------------------------------------
SET IDENTITY_INSERT SparePartCompatibilities ON;

INSERT INTO SparePartCompatibilities (Id, SparePartId, VehicleNameId, VehicleVariantId, VehicleModelYearId, CreatedAt, IsDeleted)
SELECT v.Id, v.SparePartId, v.VehicleNameId, v.VehicleVariantId, v.VehicleModelYearId, GETUTCDATE(), 0
FROM (VALUES
    -- Bugi Bosch → Toyota Camry 2.5Q 2023
    (1, 1, 1, 1, 1),
    -- Má phanh → Toyota Camry 2.5Q 2023
    (2, 2, 1, 1, 1),
    -- Gạt mưa → VinFast VF 8 Plus 2023
    (3, 3, 2, 2, 2)
) AS v(Id, SparePartId, VehicleNameId, VehicleVariantId, VehicleModelYearId)
WHERE NOT EXISTS (SELECT 1 FROM SparePartCompatibilities WHERE Id = v.Id);

SET IDENTITY_INSERT SparePartCompatibilities OFF;
GO

PRINT 'SpareParts and SparePartCompatibilities seeded successfully.';
