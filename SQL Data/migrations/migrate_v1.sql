-- ============================================================
-- AutoHub – Migration v1
-- Thêm cột Username, DateOfBirth vào Users;
-- Thêm cột ImageUrl vào Vehicles và SpareParts.
-- Chạy một lần duy nhất trước khi dùng EF migrations.
-- ============================================================

ALTER TABLE Users ADD Username     NVARCHAR(50) NULL;
ALTER TABLE Users ADD DateOfBirth  DATETIME2    NULL;
GO

UPDATE Users SET Username = 'user_' + CAST(Id AS NVARCHAR(50)) WHERE Username IS NULL;
ALTER TABLE Users ALTER COLUMN Username NVARCHAR(50) NOT NULL;
GO

ALTER TABLE Users ALTER COLUMN Email        NVARCHAR(MAX) NULL;
ALTER TABLE Users ALTER COLUMN PhoneNumber  NVARCHAR(MAX) NULL;
GO

ALTER TABLE Vehicles    ADD ImageUrl NVARCHAR(MAX) NULL;
ALTER TABLE SpareParts  ADD ImageUrl NVARCHAR(MAX) NULL;
GO

PRINT 'Migration v1 applied successfully.';
