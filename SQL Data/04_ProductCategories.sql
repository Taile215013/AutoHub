-- ============================================================
-- AutoHub – Seed: ProductCategories (Danh mục sản phẩm)
-- Yêu cầu: Bảng ProductCategories đã tồn tại.
-- An toàn để chạy lại (bỏ qua nếu Id đã tồn tại).
-- ============================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

SET IDENTITY_INSERT ProductCategories ON;

INSERT INTO ProductCategories (Id, Name, Code, CategoryType, ParentCategoryId, CreatedAt, IsDeleted)
SELECT v.Id, v.Name, v.Code, v.CategoryType, v.ParentCategoryId, GETUTCDATE(), 0
FROM (VALUES
    --  Id  Name                                Code         CategoryType   ParentId
    (1,  N'Động Cơ (Engine)',              N'ENGINE',   N'SparePart',  NULL),
    (2,  N'Phanh / An Toàn (Brake)',       N'BRAKE',    N'SparePart',  NULL),
    (3,  N'Ngoại Thất / Trang Trí (Exterior)', N'EXTERIOR', N'SparePart', NULL)
) AS v(Id, Name, Code, CategoryType, ParentCategoryId)
WHERE NOT EXISTS (
    SELECT 1 FROM ProductCategories WHERE Id = v.Id
);

SET IDENTITY_INSERT ProductCategories OFF;
GO

PRINT 'ProductCategories seeded successfully.';
