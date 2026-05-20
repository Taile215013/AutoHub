CREATE DATABASE AutoHubDb COLLATE SQL_Latin1_General_CP1_CI_AS;
GO
USE AutoHubDb;
GO

CREATE TABLE Countries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE TABLE Brands (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CountryId INT NOT NULL FOREIGN KEY REFERENCES Countries(Id),
    IsVehicleBrand BIT NOT NULL DEFAULT 0,
    IsPartBrand BIT NOT NULL DEFAULT 0,
    IsToyBrand BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE TABLE Vehicles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    BrandId INT NOT NULL FOREIGN KEY REFERENCES Brands(Id),
    VehicleType VARCHAR(20) NOT NULL,
    FuelType VARCHAR(20) NOT NULL,
    Transmission VARCHAR(20) NOT NULL,
    PurchasePrice DECIMAL(18,2) NOT NULL,
    CurrentPrice DECIMAL(18,2) NOT NULL,
    Quantity INT NULL,
    EngineType VARCHAR(50) NOT NULL,
    EngineCapacity FLOAT NULL,
    SeatingCapacity INT NOT NULL DEFAULT 1,
    Weight FLOAT NULL,
    BodyStyle NVARCHAR(50) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE TABLE VehicleColors (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VehicleId INT NOT NULL FOREIGN KEY REFERENCES Vehicles(Id) ON DELETE CASCADE,
    ColorName NVARCHAR(30) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE TABLE SpareParts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    BrandId INT NOT NULL FOREIGN KEY REFERENCES Brands(Id),
    Category NVARCHAR(50) NOT NULL,
    CostPrice DECIMAL(18,2) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    StockQuantity INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE TABLE Services (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ServiceName NVARCHAR(150) NOT NULL,
    BasePrice DECIMAL(18,2) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    VehicleType VARCHAR(20) NOT NULL,
    RequiresQuote BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    Email VARCHAR(100) NULL UNIQUE,
    PhoneNumber VARCHAR(15) NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    HouseNumber NVARCHAR(50) NULL,
    StreetName NVARCHAR(100) NOT NULL,
    Ward NVARCHAR(50) NOT NULL,
    District NVARCHAR(50) NOT NULL,
    City NVARCHAR(50) NOT NULL DEFAULT N'Hồ Chí Minh',
    RankLevel VARCHAR(20) NOT NULL DEFAULT 'Bronze',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName NVARCHAR(150) NOT NULL,
    UserId INT NULL FOREIGN KEY REFERENCES Users(Id),
    OrderDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE TABLE OrderDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL FOREIGN KEY REFERENCES Orders(Id),
    ProductType NVARCHAR(50) NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

INSERT INTO Countries (Name) VALUES (N'Japan'), (N'Germany'), (N'Italy'), (N'Slovenia');

INSERT INTO Brands (Name, CountryId, IsVehicleBrand, IsPartBrand, IsToyBrand) VALUES
(N'Honda', 1, 1, 1, 0),
(N'Toyota', 1, 1, 0, 0),
(N'Yamaha', 1, 1, 0, 0),
(N'Brembo', 3, 0, 1, 0),
(N'Akrapovic', 4, 0, 0, 1);

INSERT INTO Vehicles (Name, BrandId, VehicleType, FuelType, Transmission, PurchasePrice, CurrentPrice, Quantity, EngineType, EngineCapacity, SeatingCapacity, Weight, BodyStyle) VALUES
(N'Honda Civic', 1, 'Auto', 'Gasoline', 'Automatic', 600000000, 730000000, 5, 'I4', 1.5, 5, 1300, N'Sedan'),
(N'Yamaha Exciter', 3, 'Motorbike', 'Gasoline', 'Clutch', 35000000, 48000000, NULL, N'Xăng 4 thì', 150, 2, 115, N'Underbone');

INSERT INTO VehicleColors (VehicleId, ColorName) VALUES
(1, N'Đỏ'),
(1, N'Trắng-Đen'),
(2, N'Xanh'),
(2, N'Xám Xi Măng');

INSERT INTO Services (ServiceName, BasePrice, IsActive, VehicleType, RequiresQuote) VALUES
(N'Rửa xe bọt tuyết', 50000, 1, 'Motorbike', 0),
(N'Rửa xe bọt tuyết', 150000, 1, 'Auto', 0),
(N'Thay nhớt động cơ', 150000, 1, 'Motorbike', 0),
(N'Thay nhớt động cơ', 500000, 1, 'Auto', 0),
(N'Độ đèn tăng sáng theo yêu cầu', NULL, 1, 'Auto', 1),
(N'Sửa chữa đại tu động cơ', NULL, 1, 'Auto', 1);
