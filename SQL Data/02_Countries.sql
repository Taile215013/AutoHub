-- ============================================================
-- AutoHub – Seed: Countries (Quốc gia)
-- Yêu cầu: Bảng Countries đã tồn tại.
-- An toàn để chạy lại (bỏ qua nếu Id đã tồn tại).
-- ============================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

SET IDENTITY_INSERT Countries ON;

INSERT INTO Countries (Id, Name, CreatedAt, IsDeleted)
SELECT v.Id, v.Name, GETUTCDATE(), 0
FROM (VALUES
    (1, N'Việt Nam'),
    (2, N'Nhật Bản'),
    (3, N'Đức'),
    (4, N'Thái Lan'),
    (5, N'Ý'),
    (6, N'Trung Quốc'),
    (7, N'Indonesia'),
    (8, N'Mỹ')
) AS v(Id, Name)
WHERE NOT EXISTS (
    SELECT 1 FROM Countries WHERE Id = v.Id
);

SET IDENTITY_INSERT Countries OFF;
GO

PRINT 'Countries seeded successfully.';
