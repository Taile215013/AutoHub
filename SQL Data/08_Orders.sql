-- ============================================================
-- AutoHub – Seed: Orders + OrderDetails (Đơn hàng mẫu)
-- Yêu cầu: Users (07), Vehicles (05), SpareParts (06) đã có dữ liệu.
-- An toàn để chạy lại (bỏ qua nếu Id đã tồn tại).
-- ============================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ------------------------------------------------------------
-- 1. Orders
-- ------------------------------------------------------------
SET IDENTITY_INSERT Orders ON;

INSERT INTO Orders (Id, CustomerName, UserId, OrderDate, TotalAmount, CreatedAt, IsDeleted)
SELECT v.Id, v.CustomerName, v.UserId, v.OrderDate, v.TotalAmount, GETUTCDATE(), 0
FROM (VALUES
    -- Id  CustomerName        UserId  OrderDate                    TotalAmount
    (1, N'Nguyễn Văn A',   NULL, CAST('2026-05-10 08:30:00' AS DATETIME2), 1050000000.00),
    (2, N'Trần Thị B',     NULL, CAST('2026-05-14 10:15:00' AS DATETIME2), 1980000000.00),
    (3, N'Lê Văn C',       NULL, CAST('2026-05-18 14:00:00' AS DATETIME2),    1700000.00),
    (4, N'Nguyễn Thanh Tài', 1,  CAST('2026-05-19 16:45:00' AS DATETIME2), 1350000000.00)
) AS v(Id, CustomerName, UserId, OrderDate, TotalAmount)
WHERE NOT EXISTS (SELECT 1 FROM Orders WHERE Id = v.Id);

SET IDENTITY_INSERT Orders OFF;
GO

-- ------------------------------------------------------------
-- 2. OrderDetails
-- ------------------------------------------------------------
SET IDENTITY_INSERT OrderDetails ON;

INSERT INTO OrderDetails (Id, OrderId, ProductType, ProductId, Quantity, Price, CreatedAt, IsDeleted)
SELECT v.Id, v.OrderId, v.ProductType, v.ProductId, v.Quantity, v.Price, GETUTCDATE(), 0
FROM (VALUES
    -- Id  OrderId  ProductType   ProductId  Qty  Price
    (1, 1, N'Vehicle',   1, 1, 1050000000.00),
    (2, 2, N'Vehicle',   2, 2,  990000000.00),
    (3, 3, N'SparePart', 1, 1,     250000.00),
    (4, 3, N'SparePart', 2, 1,    1450000.00),
    (5, 4, N'Vehicle',   3, 1, 1350000000.00)
) AS v(Id, OrderId, ProductType, ProductId, Quantity, Price)
WHERE NOT EXISTS (SELECT 1 FROM OrderDetails WHERE Id = v.Id);

SET IDENTITY_INSERT OrderDetails OFF;
GO

PRINT 'Orders and OrderDetails seeded successfully.';
