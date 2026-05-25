-- Tự động tạo bảng SystemDictionaries nếu chưa tồn tại
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SystemDictionaries')
BEGIN
    CREATE TABLE SystemDictionaries (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Type NVARCHAR(50) NOT NULL,
        Code NVARCHAR(50) NOT NULL,
        Value NVARCHAR(100) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
END

-- Xóa dữ liệu cũ theo thứ tự khóa ngoại (chỉ xóa nếu bảng tồn tại)
IF OBJECT_ID('OrderDetails', 'U') IS NOT NULL DELETE FROM OrderDetails;
IF OBJECT_ID('Orders', 'U') IS NOT NULL DELETE FROM Orders;
IF OBJECT_ID('Users', 'U') IS NOT NULL DELETE FROM Users;
IF OBJECT_ID('VehicleColors', 'U') IS NOT NULL DELETE FROM VehicleColors;
IF OBJECT_ID('SpareParts', 'U') IS NOT NULL DELETE FROM SpareParts;
IF OBJECT_ID('Vehicles', 'U') IS NOT NULL DELETE FROM Vehicles;
IF OBJECT_ID('Brands', 'U') IS NOT NULL DELETE FROM Brands;
IF OBJECT_ID('Countries', 'U') IS NOT NULL DELETE FROM Countries;

-- Thiết lập lại IDENTITY (chỉ khi bảng tồn tại)
IF OBJECT_ID('OrderDetails', 'U') IS NOT NULL DBCC CHECKIDENT ('OrderDetails', RESEED, 0);
IF OBJECT_ID('Orders', 'U') IS NOT NULL DBCC CHECKIDENT ('Orders', RESEED, 0);
IF OBJECT_ID('Users', 'U') IS NOT NULL DBCC CHECKIDENT ('Users', RESEED, 0);
IF OBJECT_ID('VehicleColors', 'U') IS NOT NULL DBCC CHECKIDENT ('VehicleColors', RESEED, 0);
IF OBJECT_ID('SpareParts', 'U') IS NOT NULL DBCC CHECKIDENT ('SpareParts', RESEED, 0);
IF OBJECT_ID('Vehicles', 'U') IS NOT NULL DBCC CHECKIDENT ('Vehicles', RESEED, 0);
IF OBJECT_ID('Brands', 'U') IS NOT NULL DBCC CHECKIDENT ('Brands', RESEED, 0);
IF OBJECT_ID('Countries', 'U') IS NOT NULL DBCC CHECKIDENT ('Countries', RESEED, 0);

-- Chèn dữ liệu Từ điển Hệ thống (Master Data) bằng câu lệnh MERGE thông minh (Upsert)
;WITH SourceData (Type, Code, Value) AS (
    SELECT Type, Code, Value
    FROM (VALUES
        (N'VehicleType', N'Auto', N'Auto (Ô Tô)'),
        (N'VehicleType', N'Motorbike', N'Motorbike (Xe Máy)'),
        (N'FuelType', N'Gasoline', N'Xăng (Gasoline)'),
        (N'FuelType', N'Electric', N'Điện (Electric)'),
        (N'Transmission', N'Automatic', N'Automatic (Tự Động / Ga)'),
        (N'Transmission', N'Manual', N'Manual (Số Sàn / Cơ)'),
        (N'Transmission', N'Clutch', N'Clutch (Xe Côn Tay)'),
        (N'SparePartCategory', N'Engine', N'Động Cơ (Engine)'),
        (N'SparePartCategory', N'Brake', N'Phanh / An Toàn (Brake)'),
        (N'SparePartCategory', N'Suspension', N'Giảm Xóc (Suspension)'),
        (N'SparePartCategory', N'Lighting', N'Hệ Thống Đèn (Lighting)'),
        (N'SparePartCategory', N'Exterior', N'Ngoại Thất / Trang Trí (Exterior)'),
        (N'VehicleColor', N'Red', N'Đỏ'),
        (N'VehicleColor', N'White', N'Trắng'),
        (N'VehicleColor', N'Black', N'Đen'),
        (N'VehicleColor', N'Sand', N'Vàng Cát'),
        (N'VehicleColor', N'Blue', N'Xanh Dương'),
        (N'VehicleColor', N'Grey', N'Xám'),
        (N'VehicleColor', N'Silver', N'Bạc'),
        (N'VehicleColor', N'Orange', N'Cam'),
        (N'VehicleColor', N'Yellow', N'Vàng'),
        -- Loại Động Cơ (EngineType)
        (N'EngineType', N'I3', N'Động cơ I3 (3 xi-lanh thẳng hàng)'),
        (N'EngineType', N'I4', N'Động cơ I4 (4 xi-lanh thẳng hàng)'),
        (N'EngineType', N'I6', N'Động cơ I6 (6 xi-lanh thẳng hàng)'),
        (N'EngineType', N'V6', N'Động cơ V6'),
        (N'EngineType', N'V8', N'Động cơ V8'),
        (N'EngineType', N'V12', N'Động cơ V12'),
        (N'EngineType', N'W12', N'Động cơ W12'),
        (N'EngineType', N'Boxer', N'Động cơ Boxer (Xi-lanh đối xứng nằm ngang)'),
        (N'EngineType', N'Rotary', N'Động cơ xoay Wankel (Rotary)'),
        (N'EngineType', N'Electric', N'Động cơ thuần điện (Electric Motor)'),
        (N'EngineType', N'Hybrid', N'Động cơ lai xăng điện (Hybrid)'),
        (N'EngineType', N'SingleCylinder', N'Động cơ đơn (1 xi-lanh - Xe máy)'),
        (N'EngineType', N'TwinCylinder', N'Động cơ đôi (2 xi-lanh - Xe máy)'),
        -- Kiểu Dáng Xe (BodyStyle)
        (N'BodyStyle', N'Sedan', N'Sedan (4 Cửa truyền thống)'),
        (N'BodyStyle', N'SUV', N'SUV (Thể thao đa dụng)'),
        (N'BodyStyle', N'Crossover', N'Crossover (Gầm cao đô thị)'),
        (N'BodyStyle', N'Hatchback', N'Hatchback (Đuôi cụt)'),
        (N'BodyStyle', N'Coupe', N'Coupe (Thể thao 2 cửa)'),
        (N'BodyStyle', N'Convertible', N'Convertible (Mui trần)'),
        (N'BodyStyle', N'MPV', N'MPV / Minivan (Xe gia đình đa dụng)'),
        (N'BodyStyle', N'Pickup', N'Bán tải (Pickup)'),
        (N'BodyStyle', N'Wagon', N'Wagon (Sedan kéo dài đuôi)'),
        (N'BodyStyle', N'Scooter', N'Scooter (Xe tay ga - Xe máy)'),
        (N'BodyStyle', N'Underbone', N'Underbone (Xe số phổ thông - Xe máy)'),
        (N'BodyStyle', N'Sportbike', N'Sportbike (Mô tô thể thao - Xe máy)'),
        (N'BodyStyle', N'Nakedbike', N'Nakedbike (Mô tô đường phố - Xe máy)'),
        (N'BodyStyle', N'Cruiser', N'Cruiser (Mô tô hành trình - Xe máy)')
    ) AS TempTable(Type, Code, Value)
)
MERGE SystemDictionaries AS Target
USING SourceData AS Source
ON Target.Type = Source.Type AND Target.Code = Source.Code
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Type, Code, Value, CreatedAt, IsDeleted)
    VALUES (Source.Type, Source.Code, Source.Value, GETUTCDATE(), 0)
WHEN MATCHED AND Target.Value <> Source.Value THEN
    UPDATE SET Target.Value = Source.Value, Target.UpdatedAt = GETUTCDATE();


-- Chèn dữ liệu các Quốc gia
SET IDENTITY_INSERT Countries ON;
INSERT INTO Countries (Id, Name, CreatedAt, IsDeleted) VALUES
(1, N'Việt Nam', GETUTCDATE(), 0),
(2, N'Nhật Bản', GETUTCDATE(), 0),
(3, N'Đức', GETUTCDATE(), 0),
(4, N'Thái Lan', GETUTCDATE(), 0),
(5, N'Ý', GETUTCDATE(), 0),
(6, N'Trung Quốc', GETUTCDATE(), 0),
(7, N'Indonesia', GETUTCDATE(), 0),
(8, N'Mỹ', GETUTCDATE(), 0);
SET IDENTITY_INSERT Countries OFF;

-- Chèn dữ liệu các Thương hiệu (Brands)
SET IDENTITY_INSERT Brands ON;
INSERT INTO Brands (Id, Name, CountryId, IsVehicleBrand, IsPartBrand, IsToyBrand, CreatedAt, IsDeleted) VALUES
(1, N'VinFast', 1, 1, 1, 0, GETUTCDATE(), 0),
(2, N'Toyota', 2, 1, 1, 0, GETUTCDATE(), 0),
(3, N'BMW', 3, 1, 0, 0, GETUTCDATE(), 0),
(4, N'Bosch', 3, 0, 1, 0, GETUTCDATE(), 0),
(5, N'Ford', 8, 1, 1, 0, GETUTCDATE(), 0),
(6, N'Ferrari', 5, 1, 0, 0, GETUTCDATE(), 0),
(7, N'BYD', 6, 1, 1, 0, GETUTCDATE(), 0),
(8, N'Astra Otoparts', 7, 0, 1, 0, GETUTCDATE(), 0),
(9, N'Thai Summit', 4, 0, 1, 0, GETUTCDATE(), 0);
SET IDENTITY_INSERT Brands OFF;

-- Chèn dữ liệu các dòng xe (Vehicles)
SET IDENTITY_INSERT Vehicles ON;
INSERT INTO Vehicles (Id, Name, BrandId, VehicleType, FuelType, Transmission, PurchasePrice, CurrentPrice, Quantity, EngineType, EngineCapacity, SeatingCapacity, Weight, BodyStyle, CreatedAt, IsDeleted) VALUES
(1, N'Toyota Camry 2.5Q', 2, N'Auto', N'Gasoline', N'Automatic', 950000000.00, 1050000000.00, 3, N'I4 Dual VVT-i', 2.5, 5, 1560, N'Sedan', GETUTCDATE(), 0),
(2, N'VinFast VF 8 Plus', 1, N'Auto', N'Electric', N'Automatic', 880000000.00, 990000000.00, 5, N'Dual Electric Motor', 0, 5, 2600, N'SUV', GETUTCDATE(), 0),
(3, N'BMW 320i Sport Line', 3, N'Auto', N'Gasoline', N'Automatic', 1200000000.00, 1350000000.00, 2, N'TwinPower Turbo I4', 2.0, 5, 1500, N'Sedan', GETUTCDATE(), 0),
(4, N'Ford Ranger Wildtrak', 5, N'Auto', N'Gasoline', N'Automatic', 720000000.00, 820000000.00, 4, N'Bi-Turbo I4', 2.0, 5, 2238, N'Pickup', GETUTCDATE(), 0),
(5, N'Ferrari F8 Tributo', 6, N'Auto', N'Gasoline', N'Automatic', 18000000000.00, 20500000000.00, 1, N'V8 Twin-Turbo', 3.9, 2, 1435, N'Coupe', GETUTCDATE(), 0),
(6, N'BYD Atto 3', 7, N'Auto', N'Electric', N'Automatic', 650000000.00, 760000000.00, 6, N'Single Electric Motor', 0, 5, 1750, N'Crossover', GETUTCDATE(), 0);
SET IDENTITY_INSERT Vehicles OFF;

-- Chèn dữ liệu màu sắc xe (VehicleColors)
SET IDENTITY_INSERT VehicleColors ON;
INSERT INTO VehicleColors (Id, VehicleId, ColorName, CreatedAt, IsDeleted) VALUES
(1, 1, N'Đen', GETUTCDATE(), 0),
(2, 1, N'Trắng', GETUTCDATE(), 0),
(3, 1, N'Vàng cát', GETUTCDATE(), 0),
(4, 2, N'Đỏ Premium', GETUTCDATE(), 0),
(5, 2, N'Xanh dương', GETUTCDATE(), 0),
(6, 2, N'Xám', GETUTCDATE(), 0),
(7, 3, N'Trắng Ngọc Trai', GETUTCDATE(), 0),
(8, 3, N'Đen Sapphire', GETUTCDATE(), 0),
(9, 4, N'Cam Cyber', GETUTCDATE(), 0),
(10, 4, N'Đen', GETUTCDATE(), 0),
(11, 5, N'Đỏ Rosso Corsa', GETUTCDATE(), 0),
(12, 5, N'Vàng Modena', GETUTCDATE(), 0),
(13, 6, N'Xanh Năng Động', GETUTCDATE(), 0),
(14, 6, N'Xám', GETUTCDATE(), 0);
SET IDENTITY_INSERT VehicleColors OFF;

-- Chèn dữ liệu phụ tùng (SpareParts)
SET IDENTITY_INSERT SpareParts ON;
INSERT INTO SpareParts (Id, Name, BrandId, Category, CostPrice, Price, StockQuantity, CreatedAt, IsDeleted) VALUES
(1, N'Bugi Bosch Bạch Kim', 4, N'Engine', 120000.00, 250000.00, 100, GETUTCDATE(), 0),
(2, N'Má phanh trước Toyota Camry', 2, N'Brake', 850000.00, 1450000.00, 40, GETUTCDATE(), 0),
(3, N'Gạt mưa silicon VinFast VF8', 1, N'Exterior', 300000.00, 550000.00, 50, GETUTCDATE(), 0),
(4, N'Lọc gió động cơ K&N', 5, N'Engine', 450000.00, 800000.00, 30, GETUTCDATE(), 0),
(5, N'Dầu động cơ cao cấp Liqui Moly 5W-30', 4, N'Engine', 600000.00, 950000.00, 60, GETUTCDATE(), 0),
(6, N'Lọc dầu Astra Indonesia', 8, N'Engine', 90000.00, 180000.00, 120, GETUTCDATE(), 0),
(7, N'Khung sườn Thai Summit', 9, N'Exterior', 2500000.00, 4200000.00, 15, GETUTCDATE(), 0);
SET IDENTITY_INSERT SpareParts OFF;

-- Chèn dữ liệu Người dùng mẫu (Users)
-- Password: 123456 → SHA256 hash
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (Id, FirstName, LastName, Gender, Email, PhoneNumber, PasswordHash, HouseNumber, StreetName, Ward, District, City, RankLevel, CreatedAt, IsDeleted) VALUES
(1, N'Tài', N'Nguyễn Thanh', N'Nam', N'tai.nguyen@autohub.vn', N'0912345678', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'123', N'Đường Quang Trung', N'Phường 10', N'Quận Gò Vấp', N'Hồ Chí Minh', N'Gold', GETUTCDATE(), 0);
SET IDENTITY_INSERT Users OFF;

-- Chèn dữ liệu Đơn hàng mẫu (Orders)
SET IDENTITY_INSERT Orders ON;
INSERT INTO Orders (Id, CustomerName, UserId, OrderDate, TotalAmount, CreatedAt, IsDeleted) VALUES
(1, N'Nguyễn Văn A', NULL, '2026-05-10 08:30:00', 1050000000.00, GETUTCDATE(), 0),
(2, N'Trần Thị B', NULL, '2026-05-14 10:15:00', 1980000000.00, GETUTCDATE(), 0),
(3, N'Lê Văn C', NULL, '2026-05-18 14:00:00', 1700000.00, GETUTCDATE(), 0),
(4, N'Nguyễn Thanh Tài', 1, '2026-05-19 16:45:00', 1350000000.00, GETUTCDATE(), 0);
SET IDENTITY_INSERT Orders OFF;

-- Chèn dữ liệu Chi tiết đơn hàng mẫu (OrderDetails)
SET IDENTITY_INSERT OrderDetails ON;
INSERT INTO OrderDetails (Id, OrderId, ProductType, ProductId, Quantity, Price, CreatedAt, IsDeleted) VALUES
(1, 1, N'Vehicle', 1, 1, 1050000000.00, GETUTCDATE(), 0),
(2, 2, N'Vehicle', 2, 2, 990000000.00, GETUTCDATE(), 0),
(3, 3, N'SparePart', 1, 1, 250000.00, GETUTCDATE(), 0),
(4, 3, N'SparePart', 2, 1, 1450000.00, GETUTCDATE(), 0),
(5, 4, N'Vehicle', 3, 1, 1350000000.00, GETUTCDATE(), 0);
SET IDENTITY_INSERT OrderDetails OFF;
