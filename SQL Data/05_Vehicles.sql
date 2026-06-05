-- ============================================================
-- AutoHub – Seed: Vehicles (Xe + tên / biến thể / năm / màu)
-- Yêu cầu: Brands (03) đã có dữ liệu.
-- Thứ tự chèn: VehicleNames → VehicleVariants → VehicleModelYears
--              → Vehicles → VehicleColors
-- An toàn để chạy lại (bỏ qua nếu Id đã tồn tại).
-- ============================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ------------------------------------------------------------
-- 1. VehicleNames (Dòng xe)
-- ------------------------------------------------------------
SET IDENTITY_INSERT VehicleNames ON;

INSERT INTO VehicleNames (Id, BrandId, Name, NormalizedName, VehicleType, BodyStyle, CreatedAt, IsDeleted)
SELECT v.Id, v.BrandId, v.Name, v.NormalizedName, v.VehicleType, v.BodyStyle, GETUTCDATE(), 0
FROM (VALUES
    (1, 2, N'Camry',     N'camry',     N'Auto', N'Sedan'),
    (2, 1, N'VF 8',      N'vf8',       N'Auto', N'SUV'),
    (3, 3, N'3 Series',  N'3series',   N'Auto', N'Sedan'),
    (4, 5, N'Ranger',    N'ranger',    N'Auto', N'Pickup'),
    (5, 6, N'F8 Tributo',N'f8tributo', N'Auto', N'Coupe'),
    (6, 7, N'Atto 3',    N'atto3',     N'Auto', N'Crossover')
) AS v(Id, BrandId, Name, NormalizedName, VehicleType, BodyStyle)
WHERE NOT EXISTS (SELECT 1 FROM VehicleNames WHERE Id = v.Id);

SET IDENTITY_INSERT VehicleNames OFF;
GO

-- ------------------------------------------------------------
-- 2. VehicleVariants (Phiên bản / Trim)
-- ------------------------------------------------------------
SET IDENTITY_INSERT VehicleVariants ON;

INSERT INTO VehicleVariants (Id, VehicleNameId, Name, EngineType, EngineCapacity, CreatedAt, IsDeleted)
SELECT v.Id, v.VehicleNameId, v.Name, v.EngineType, v.EngineCapacity, GETUTCDATE(), 0
FROM (VALUES
    (1, 1, N'2.5Q',           N'I4 Dual VVT-i',         2.5),
    (2, 2, N'Plus',           N'Dual Electric Motor',   0.0),
    (3, 3, N'320i Sport Line',N'TwinPower Turbo I4',     2.0),
    (4, 4, N'Wildtrak',       N'Bi-Turbo I4',            2.0),
    (5, 5, N'Base',           N'V8 Twin-Turbo',          3.9),
    (6, 6, N'Premium',        N'Single Electric Motor', 0.0)
) AS v(Id, VehicleNameId, Name, EngineType, EngineCapacity)
WHERE NOT EXISTS (SELECT 1 FROM VehicleVariants WHERE Id = v.Id);

SET IDENTITY_INSERT VehicleVariants OFF;
GO

-- ------------------------------------------------------------
-- 3. VehicleModelYears (Năm model)
-- ------------------------------------------------------------
SET IDENTITY_INSERT VehicleModelYears ON;

INSERT INTO VehicleModelYears (Id, VehicleVariantId, Year, CreatedAt, IsDeleted)
SELECT v.Id, v.VehicleVariantId, v.Year, GETUTCDATE(), 0
FROM (VALUES
    (1, 1, 2023),
    (2, 2, 2023),
    (3, 3, 2022),
    (4, 4, 2023),
    (5, 5, 2021),
    (6, 6, 2024)
) AS v(Id, VehicleVariantId, Year)
WHERE NOT EXISTS (SELECT 1 FROM VehicleModelYears WHERE Id = v.Id);

SET IDENTITY_INSERT VehicleModelYears OFF;
GO

-- ------------------------------------------------------------
-- 4. Vehicles (Xe hàng hoá / tồn kho)
-- ------------------------------------------------------------
SET IDENTITY_INSERT Vehicles ON;

INSERT INTO Vehicles (
    Id, Name, BrandId, VehicleNameId, VehicleVariantId, VehicleModelYearId,
    VehicleType, FuelType, Transmission,
    PurchasePrice, CurrentPrice, Quantity,
    EngineType, EngineCapacity, SeatingCapacity, Weight, BodyStyle,
    CreatedAt, IsDeleted
)
SELECT
    v.Id, v.Name, v.BrandId, v.VehicleNameId, v.VehicleVariantId, v.VehicleModelYearId,
    v.VehicleType, v.FuelType, v.Transmission,
    v.PurchasePrice, v.CurrentPrice, v.Quantity,
    v.EngineType, v.EngineCapacity, v.SeatingCapacity, v.Weight, v.BodyStyle,
    GETUTCDATE(), 0
FROM (VALUES
    -- Id  Name                         BrandId  NameId  VariantId  YearId  VType   Fuel        Trans        PurchasePrice      CurrentPrice       Qty  EngineType              EngCap  Seats  Weight  BodyStyle
    (1, N'Toyota Camry 2.5Q 2023',  2, 1, 1, 1, N'Auto', N'Gasoline', N'Automatic',  950000000.00, 1050000000.00, 3, N'I4 Dual VVT-i',         2.5, 5, 1560, N'Sedan'),
    (2, N'VinFast VF 8 Plus',       1, 2, 2, 2, N'Auto', N'Electric', N'Automatic',  880000000.00,  990000000.00, 5, N'Dual Electric Motor',   0.0, 5, 2600, N'SUV'),
    (3, N'BMW 320i Sport Line',      3, 3, 3, 3, N'Auto', N'Gasoline', N'Automatic', 1200000000.00, 1350000000.00, 2, N'TwinPower Turbo I4',    2.0, 5, 1500, N'Sedan'),
    (4, N'Ford Ranger Wildtrak',     5, 4, 4, 4, N'Auto', N'Gasoline', N'Automatic',  720000000.00,  820000000.00, 4, N'Bi-Turbo I4',           2.0, 5, 2238, N'Pickup'),
    (5, N'Ferrari F8 Tributo',       6, 5, 5, 5, N'Auto', N'Gasoline', N'Automatic',18000000000.00,20500000000.00, 1, N'V8 Twin-Turbo',         3.9, 2, 1435, N'Coupe'),
    (6, N'BYD Atto 3',               7, 6, 6, 6, N'Auto', N'Electric', N'Automatic',  650000000.00,  760000000.00, 6, N'Single Electric Motor', 0.0, 5, 1750, N'Crossover')
) AS v(Id, Name, BrandId, VehicleNameId, VehicleVariantId, VehicleModelYearId,
       VehicleType, FuelType, Transmission,
       PurchasePrice, CurrentPrice, Quantity,
       EngineType, EngineCapacity, SeatingCapacity, Weight, BodyStyle)
WHERE NOT EXISTS (SELECT 1 FROM Vehicles WHERE Id = v.Id);

SET IDENTITY_INSERT Vehicles OFF;
GO

-- ------------------------------------------------------------
-- 5. VehicleColors (Màu sắc xe)
-- ------------------------------------------------------------
SET IDENTITY_INSERT VehicleColors ON;

INSERT INTO VehicleColors (Id, VehicleId, ColorName, CreatedAt, IsDeleted)
SELECT v.Id, v.VehicleId, v.ColorName, GETUTCDATE(), 0
FROM (VALUES
    -- Toyota Camry
    (1,  1, N'Đen'),
    (2,  1, N'Trắng'),
    (3,  1, N'Vàng cát'),
    -- VinFast VF 8
    (4,  2, N'Đỏ Premium'),
    (5,  2, N'Xanh dương'),
    (6,  2, N'Xám'),
    -- BMW 3 Series
    (7,  3, N'Trắng Ngọc Trai'),
    (8,  3, N'Đen Sapphire'),
    -- Ford Ranger
    (9,  4, N'Cam Cyber'),
    (10, 4, N'Đen'),
    -- Ferrari F8
    (11, 5, N'Đỏ Rosso Corsa'),
    (12, 5, N'Vàng Modena'),
    -- BYD Atto 3
    (13, 6, N'Xanh Năng Động'),
    (14, 6, N'Xám')
) AS v(Id, VehicleId, ColorName)
WHERE NOT EXISTS (SELECT 1 FROM VehicleColors WHERE Id = v.Id);

SET IDENTITY_INSERT VehicleColors OFF;
GO

PRINT 'Vehicles (Names / Variants / ModelYears / Vehicles / Colors) seeded successfully.';
