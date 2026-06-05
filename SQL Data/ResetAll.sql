-- ============================================================
-- AutoHub – ResetAll.sql
-- XÓA TOÀN BỘ dữ liệu và reset IDENTITY về 0.
-- ⚠️  CHỈ dùng trên môi trường DEV / TEST.
-- Sau khi chạy xong, chạy RunAll.sql để nạp lại.
-- ============================================================

-- Xóa theo thứ tự ngược (con trước, cha sau)
IF OBJECT_ID('OrderDetails',             'U') IS NOT NULL DELETE FROM OrderDetails;
IF OBJECT_ID('Orders',                   'U') IS NOT NULL DELETE FROM Orders;
IF OBJECT_ID('Users',                    'U') IS NOT NULL DELETE FROM Users;
IF OBJECT_ID('SparePartCompatibilities', 'U') IS NOT NULL DELETE FROM SparePartCompatibilities;
IF OBJECT_ID('SpareParts',               'U') IS NOT NULL DELETE FROM SpareParts;
IF OBJECT_ID('VehicleColors',            'U') IS NOT NULL DELETE FROM VehicleColors;
IF OBJECT_ID('Vehicles',                 'U') IS NOT NULL DELETE FROM Vehicles;
IF OBJECT_ID('VehicleModelYears',        'U') IS NOT NULL DELETE FROM VehicleModelYears;
IF OBJECT_ID('VehicleVariants',          'U') IS NOT NULL DELETE FROM VehicleVariants;
IF OBJECT_ID('VehicleNames',             'U') IS NOT NULL DELETE FROM VehicleNames;
IF OBJECT_ID('SpareParts',               'U') IS NOT NULL DELETE FROM SpareParts;
IF OBJECT_ID('ProductCategories',        'U') IS NOT NULL DELETE FROM ProductCategories;
IF OBJECT_ID('Brands',                   'U') IS NOT NULL DELETE FROM Brands;
IF OBJECT_ID('Countries',                'U') IS NOT NULL DELETE FROM Countries;
GO

-- Reset IDENTITY về 0 (bắt đầu lại từ 1)
IF OBJECT_ID('OrderDetails',             'U') IS NOT NULL DBCC CHECKIDENT ('OrderDetails',             RESEED, 0);
IF OBJECT_ID('Orders',                   'U') IS NOT NULL DBCC CHECKIDENT ('Orders',                   RESEED, 0);
IF OBJECT_ID('Users',                    'U') IS NOT NULL DBCC CHECKIDENT ('Users',                    RESEED, 0);
IF OBJECT_ID('SparePartCompatibilities', 'U') IS NOT NULL DBCC CHECKIDENT ('SparePartCompatibilities', RESEED, 0);
IF OBJECT_ID('SpareParts',               'U') IS NOT NULL DBCC CHECKIDENT ('SpareParts',               RESEED, 0);
IF OBJECT_ID('VehicleColors',            'U') IS NOT NULL DBCC CHECKIDENT ('VehicleColors',            RESEED, 0);
IF OBJECT_ID('Vehicles',                 'U') IS NOT NULL DBCC CHECKIDENT ('Vehicles',                 RESEED, 0);
IF OBJECT_ID('VehicleModelYears',        'U') IS NOT NULL DBCC CHECKIDENT ('VehicleModelYears',        RESEED, 0);
IF OBJECT_ID('VehicleVariants',          'U') IS NOT NULL DBCC CHECKIDENT ('VehicleVariants',          RESEED, 0);
IF OBJECT_ID('VehicleNames',             'U') IS NOT NULL DBCC CHECKIDENT ('VehicleNames',             RESEED, 0);
IF OBJECT_ID('ProductCategories',        'U') IS NOT NULL DBCC CHECKIDENT ('ProductCategories',        RESEED, 0);
IF OBJECT_ID('Brands',                   'U') IS NOT NULL DBCC CHECKIDENT ('Brands',                   RESEED, 0);
IF OBJECT_ID('Countries',                'U') IS NOT NULL DBCC CHECKIDENT ('Countries',                RESEED, 0);
GO

PRINT '=== All tables reset. Run RunAll.sql to re-seed. ===';
