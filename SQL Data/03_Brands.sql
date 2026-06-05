-- ============================================================
-- AutoHub – Seed: Brands (Thương hiệu)
-- Yêu cầu: Bảng Countries đã có dữ liệu (02_Countries.sql).
-- An toàn để chạy lại (bỏ qua nếu Id đã tồn tại).
-- ============================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

SET IDENTITY_INSERT Brands ON;

INSERT INTO Brands (Id, Name, CountryId, IsVehicleBrand, IsPartBrand, IsToyBrand, CreatedAt, IsDeleted)
SELECT v.Id, v.Name, v.CountryId, v.IsVehicleBrand, v.IsPartBrand, v.IsToyBrand, GETUTCDATE(), 0
FROM (VALUES
    --  Id  Name                     CountryId  IsVehicle  IsPart  IsToy
    (1,  N'VinFast',          1,         1,         1,      0),
    (2,  N'Toyota',           2,         1,         1,      0),
    (3,  N'BMW',              3,         1,         0,      0),
    (4,  N'Bosch',            3,         0,         1,      0),
    (5,  N'Ford',             8,         1,         1,      0),
    (6,  N'Ferrari',          5,         1,         0,      0),
    (7,  N'BYD',              6,         1,         1,      0),
    (8,  N'Astra Otoparts',   7,         0,         1,      0),
    (9,  N'Thai Summit',      4,         0,         1,      0)
) AS v(Id, Name, CountryId, IsVehicleBrand, IsPartBrand, IsToyBrand)
WHERE NOT EXISTS (
    SELECT 1 FROM Brands WHERE Id = v.Id
);

SET IDENTITY_INSERT Brands OFF;
GO

PRINT 'Brands seeded successfully.';
