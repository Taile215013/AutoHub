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
        CreatedAt DATETIME     NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME     NULL,
        IsDeleted BIT          NOT NULL DEFAULT 0
    );
END
GO

;WITH SourceData (Type, Code, Value) AS (
    SELECT Type, Code, Value
    FROM (VALUES
        -- VehicleType
        (N'VehicleType', N'Auto',       N'Auto (Ô Tô)'),
        (N'VehicleType', N'Motorbike',  N'Motorbike (Xe Máy)'),

        -- FuelType
        (N'FuelType', N'Gasoline', N'Xăng (Gasoline)'),
        (N'FuelType', N'Electric', N'Điện (Electric)'),

        -- Transmission
        (N'Transmission', N'Automatic', N'Automatic (Tự Động / Ga)'),
        (N'Transmission', N'Manual',    N'Manual (Số Sàn / Cơ)'),
        (N'Transmission', N'Clutch',    N'Clutch (Xe Côn Tay)'),

        -- SparePartCategory
        (N'SparePartCategory', N'Engine',     N'Động Cơ (Engine)'),
        (N'SparePartCategory', N'Brake',      N'Phanh / An Toàn (Brake)'),
        (N'SparePartCategory', N'Suspension', N'Giảm Xóc (Suspension)'),
        (N'SparePartCategory', N'Lighting',   N'Hệ Thống Đèn (Lighting)'),
        (N'SparePartCategory', N'Exterior',   N'Ngoại Thất / Trang Trí (Exterior)'),

        -- VehicleColor
        (N'VehicleColor', N'Red',    N'Đỏ'),
        (N'VehicleColor', N'White',  N'Trắng'),
        (N'VehicleColor', N'Black',  N'Đen'),
        (N'VehicleColor', N'Sand',   N'Vàng Cát'),
        (N'VehicleColor', N'Blue',   N'Xanh Dương'),
        (N'VehicleColor', N'Grey',   N'Xám'),
        (N'VehicleColor', N'Silver', N'Bạc'),
        (N'VehicleColor', N'Orange', N'Cam'),
        (N'VehicleColor', N'Yellow', N'Vàng'),

        -- EngineType
        (N'EngineType', N'I3',            N'Động cơ I3 (3 xi-lanh thẳng hàng)'),
        (N'EngineType', N'I4',            N'Động cơ I4 (4 xi-lanh thẳng hàng)'),
        (N'EngineType', N'I6',            N'Động cơ I6 (6 xi-lanh thẳng hàng)'),
        (N'EngineType', N'V6',            N'Động cơ V6'),
        (N'EngineType', N'V8',            N'Động cơ V8'),
        (N'EngineType', N'V12',           N'Động cơ V12'),
        (N'EngineType', N'W12',           N'Động cơ W12'),
        (N'EngineType', N'Boxer',         N'Động cơ Boxer (Xi-lanh đối xứng nằm ngang)'),
        (N'EngineType', N'Rotary',        N'Động cơ xoay Wankel (Rotary)'),
        (N'EngineType', N'Electric',      N'Động cơ thuần điện (Electric Motor)'),
        (N'EngineType', N'Hybrid',        N'Động cơ lai xăng điện (Hybrid)'),
        (N'EngineType', N'SingleCylinder',N'Động cơ đơn (1 xi-lanh - Xe máy)'),
        (N'EngineType', N'TwinCylinder',  N'Động cơ đôi (2 xi-lanh - Xe máy)'),

        -- BodyStyle
        (N'BodyStyle', N'Sedan',      N'Sedan (4 Cửa truyền thống)'),
        (N'BodyStyle', N'SUV',        N'SUV (Thể thao đa dụng)'),
        (N'BodyStyle', N'Crossover',  N'Crossover (Gầm cao đô thị)'),
        (N'BodyStyle', N'Hatchback',  N'Hatchback (Đuôi cụt)'),
        (N'BodyStyle', N'Coupe',      N'Coupe (Thể thao 2 cửa)'),
        (N'BodyStyle', N'Convertible',N'Convertible (Mui trần)'),
        (N'BodyStyle', N'MPV',        N'MPV / Minivan (Xe gia đình đa dụng)'),
        (N'BodyStyle', N'Pickup',     N'Bán tải (Pickup)'),
        (N'BodyStyle', N'Wagon',      N'Wagon (Sedan kéo dài đuôi)'),
        (N'BodyStyle', N'Scooter',    N'Scooter (Xe tay ga - Xe máy)'),
        (N'BodyStyle', N'Underbone',  N'Underbone (Xe số phổ thông - Xe máy)'),
        (N'BodyStyle', N'Sportbike',  N'Sportbike (Mô tô thể thao - Xe máy)'),
        (N'BodyStyle', N'Nakedbike',  N'Nakedbike (Mô tô đường phố - Xe máy)'),
        (N'BodyStyle', N'Cruiser',    N'Cruiser (Mô tô hành trình - Xe máy)'),

        -- EmployeePosition
        (N'EmployeePosition', N'GiamDoc',       N'Giám Đốc'),
        (N'EmployeePosition', N'TruongPhongKD', N'Trưởng Phòng Kinh Doanh'),
        (N'EmployeePosition', N'NhanVienKD',    N'Nhân Viên Kinh Doanh'),
        (N'EmployeePosition', N'KyThuatVien',   N'Kỹ Thuật Viên'),
        (N'EmployeePosition', N'LeTan',         N'Lễ Tân'),
        (N'EmployeePosition', N'KeToan',        N'Kế Toán'),
        (N'EmployeePosition', N'BaoVe',         N'Bảo Vệ'),
        (N'EmployeePosition', N'Marketing',     N'Marketing'),
        (N'EmployeePosition', N'Media',         N'Media / Content'),
        (N'EmployeePosition', N'Sales',         N'Sales'),
        (N'EmployeePosition', N'TruongPhongKT', N'Trưởng Phòng Kỹ Thuật'),
        (N'EmployeePosition', N'NhanVienKho',   N'Nhân Viên Kho')
    ) AS T(Type, Code, Value)
)
MERGE SystemDictionaries AS Target
USING SourceData AS Source
    ON Target.Type = Source.Type AND Target.Code = Source.Code
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Type, Code, Value, CreatedAt, IsDeleted)
    VALUES (Source.Type, Source.Code, Source.Value, GETUTCDATE(), 0)
WHEN MATCHED AND Target.Value <> Source.Value THEN
    UPDATE SET Target.Value = Source.Value, Target.UpdatedAt = GETUTCDATE();
GO

PRINT 'SystemDictionaries seeded successfully.';
