-- ============================================================
-- AutoHub – Migration v2
-- Xóa constraint unique trùng lặp trên cột Email / PhoneNumber,
-- sau đó cho phép NULL trên cả hai cột.
-- Chạy một lần duy nhất sau migrate_v1.
-- ============================================================

ALTER TABLE Users DROP CONSTRAINT UQ__Users__85FB4E3800DBE1A1;
ALTER TABLE Users DROP CONSTRAINT UQ__Users__A9D10534A01D8565;
GO

ALTER TABLE Users ALTER COLUMN Email        NVARCHAR(MAX) NULL;
ALTER TABLE Users ALTER COLUMN PhoneNumber  NVARCHAR(MAX) NULL;
GO

PRINT 'Migration v2 applied successfully.';
