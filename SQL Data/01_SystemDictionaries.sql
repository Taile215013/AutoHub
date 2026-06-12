-- ============================================================
-- AutoHub – Seed: SystemDictionaries (Master Data)
-- Chạy file này để nạp toàn bộ dữ liệu từ điển hệ thống.
-- An toàn để chạy lại nhiều lần (MERGE / upsert).
-- ============================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- Tự động tạo bảng SystemDictionaries nếu chưa tồn tại
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SystemDictionaries')
BEGIN
    CREATE TABLE SystemDictionaries (
        Id       INT IDENTITY(1,1) PRIMARY KEY,
        Type     NVARCHAR(50)  NOT NULL,
        Code     NVARCHAR(50)  NOT NULL,
        Value    NVARCHAR(100) NOT NULL,
        ParentCode NVARCHAR(50) NULL,
        CreatedAt DATETIME     NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME     NULL,
        IsDeleted BIT          NOT NULL DEFAULT 0
    );
END
GO

;WITH SourceData (Type, Code, Value, ParentCode) AS (
    SELECT Type, Code, Value, ParentCode
    FROM (VALUES
        -- VehicleType
        (N'VehicleType', N'Auto',       N'Auto (Ô Tô)', NULL),
        (N'VehicleType', N'Motorbike',  N'Motorbike (Xe Máy)', NULL),

        -- FuelType
        (N'FuelType', N'Gasoline', N'Xăng (Gasoline)', NULL),
        (N'FuelType', N'Electric', N'Điện (Electric)', NULL),

        -- Transmission
        (N'Transmission', N'Automatic', N'Automatic (Tự Động / Ga)', NULL),
        (N'Transmission', N'Manual',    N'Manual (Số Sàn / Cơ)', NULL),
        (N'Transmission', N'Clutch',    N'Clutch (Xe Côn Tay)', NULL),

        -- SparePartCategory
        (N'SparePartCategory', N'Engine',     N'Động Cơ (Engine)', NULL),
        (N'SparePartCategory', N'Brake',      N'Phanh / An Toàn (Brake)', NULL),
        (N'SparePartCategory', N'Suspension', N'Giảm Xóc (Suspension)', NULL),
        (N'SparePartCategory', N'Lighting',   N'Hệ Thống Đèn (Lighting)', NULL),
        (N'SparePartCategory', N'Exterior',   N'Ngoại Thất / Trang Trí (Exterior)', NULL),

        -- VehicleColor
        (N'VehicleColor', N'Red',    N'Đỏ', NULL),
        (N'VehicleColor', N'White',  N'Trắng', NULL),
        (N'VehicleColor', N'Black',  N'Đen', NULL),
        (N'VehicleColor', N'Sand',   N'Vàng Cát', NULL),
        (N'VehicleColor', N'Blue',   N'Xanh Dương', NULL),
        (N'VehicleColor', N'Grey',   N'Xám', NULL),
        (N'VehicleColor', N'Silver', N'Bạc', NULL),
        (N'VehicleColor', N'Orange', N'Cam', NULL),
        (N'VehicleColor', N'Yellow', N'Vàng', NULL),

        -- EngineType
        (N'EngineType', N'I3',            N'Động cơ I3 (3 xi-lanh thẳng hàng)', N'Auto'),
        (N'EngineType', N'I4',            N'Động cơ I4 (4 xi-lanh thẳng hàng)', N'Auto'),
        (N'EngineType', N'I6',            N'Động cơ I6 (6 xi-lanh thẳng hàng)', N'Auto'),
        (N'EngineType', N'V6',            N'Động cơ V6', N'Auto'),
        (N'EngineType', N'V8',            N'Động cơ V8', N'Auto'),
        (N'EngineType', N'V12',           N'Động cơ V12', N'Auto'),
        (N'EngineType', N'W12',           N'Động cơ W12', N'Auto'),
        (N'EngineType', N'Boxer',         N'Động cơ Boxer (Xi-lanh đối xứng nằm ngang)', N'Auto'),
        (N'EngineType', N'Rotary',        N'Động cơ xoay Wankel (Rotary)', N'Auto'),
        (N'EngineType', N'Electric',      N'Động cơ thuần điện (Electric Motor)', N'Auto'),
        (N'EngineType', N'Hybrid',        N'Động cơ lai xăng điện (Hybrid)', N'Auto'),
        (N'EngineType', N'SingleCylinder',N'Động cơ đơn (1 xi-lanh - Xe máy)', N'Motorbike'),
        (N'EngineType', N'TwinCylinder',  N'Động cơ đôi (2 xi-lanh - Xe máy)', N'Motorbike'),

        -- BodyStyle
        (N'BodyStyle', N'Sedan',      N'Sedan (4 Cửa truyền thống)', N'Auto'),
        (N'BodyStyle', N'SUV',        N'SUV (Thể thao đa dụng)', N'Auto'),
        (N'BodyStyle', N'Crossover',  N'Crossover (Gầm cao đô thị)', N'Auto'),
        (N'BodyStyle', N'Hatchback',  N'Hatchback (Đuôi cụt)', N'Auto'),
        (N'BodyStyle', N'Coupe',      N'Coupe (Thể thao 2 cửa)', N'Auto'),
        (N'BodyStyle', N'Convertible',N'Convertible (Mui trần)', N'Auto'),
        (N'BodyStyle', N'MPV',        N'MPV / Minivan (Xe gia đình đa dụng)', N'Auto'),
        (N'BodyStyle', N'Pickup',     N'Bán tải (Pickup)', N'Auto'),
        (N'BodyStyle', N'Wagon',      N'Wagon (Sedan kéo dài đuôi)', N'Auto'),
        (N'BodyStyle', N'Scooter',    N'Scooter (Xe tay ga - Xe máy)', N'Motorbike'),
        (N'BodyStyle', N'Underbone',  N'Underbone (Xe số phổ thông - Xe máy)', N'Motorbike'),
        (N'BodyStyle', N'Sportbike',  N'Sportbike (Mô tô thể thao - Xe máy)', N'Motorbike'),
        (N'BodyStyle', N'Nakedbike',  N'Nakedbike (Mô tô đường phố - Xe máy)', N'Motorbike'),
        (N'BodyStyle', N'Cruiser',    N'Cruiser (Mô tô hành trình - Xe máy)', N'Motorbike'),

        -- EmployeePosition
        (N'EmployeePosition', N'GiamDoc',       N'Giám Đốc', NULL),
        (N'EmployeePosition', N'TruongPhongKD', N'Trưởng Phòng Kinh Doanh', NULL),
        (N'EmployeePosition', N'NhanVienKD',    N'Nhân Viên Kinh Doanh', NULL),
        (N'EmployeePosition', N'KyThuatVien',   N'Kỹ Thuật Viên', NULL),
        (N'EmployeePosition', N'LeTan',         N'Lễ Tân', NULL),
        (N'EmployeePosition', N'KeToan',        N'Kế Toán', NULL),
        (N'EmployeePosition', N'BaoVe',         N'Bảo Vệ', NULL),
        (N'EmployeePosition', N'Marketing',     N'Marketing', NULL),
        (N'EmployeePosition', N'Media',         N'Media / Content', NULL),
        (N'EmployeePosition', N'Sales',         N'Sales', NULL),
        (N'EmployeePosition', N'TruongPhongKT', N'Trưởng Phòng Kỹ Thuật', NULL),
        (N'EmployeePosition', N'NhanVienKho',   N'Nhân Viên Kho', NULL)
    ) AS T(Type, Code, Value, ParentCode)
)
MERGE SystemDictionaries AS Target
USING SourceData AS Source
    ON Target.Type = Source.Type AND Target.Code = Source.Code
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Type, Code, Value, ParentCode, CreatedAt, IsDeleted)
    VALUES (Source.Type, Source.Code, Source.Value, Source.ParentCode, GETUTCDATE(), 0)
WHEN MATCHED AND (Target.Value <> Source.Value OR ISNULL(Target.ParentCode, '') <> ISNULL(Source.ParentCode, '')) THEN
    UPDATE SET Target.Value = Source.Value, Target.ParentCode = Source.ParentCode, Target.UpdatedAt = GETUTCDATE();
GO

PRINT 'SystemDictionaries seeded successfully.';
