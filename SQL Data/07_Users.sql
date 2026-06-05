-- ============================================================
-- AutoHub – Seed: Users (Người dùng mẫu)
-- Password cho tất cả tài khoản mẫu: 123456
-- PasswordHash = SHA-256("123456")
-- An toàn để chạy lại (bỏ qua nếu Id đã tồn tại).
-- ============================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

SET IDENTITY_INSERT Users ON;

INSERT INTO Users (
    Id, Username, FirstName, LastName, Gender,
    Email, PhoneNumber, PasswordHash,
    HouseNumber, StreetName, Ward, District, City,
    RankLevel, CreatedAt, IsDeleted
)
SELECT
    v.Id, v.Username, v.FirstName, v.LastName, v.Gender,
    v.Email, v.PhoneNumber, v.PasswordHash,
    v.HouseNumber, v.StreetName, v.Ward, v.District, v.City,
    v.RankLevel, GETUTCDATE(), 0
FROM (VALUES
    (
        1,
        N'tainguyen',
        N'Tài', N'Nguyễn Thanh',
        N'Nam',
        N'tai.nguyen@autohub.vn', N'0912345678',
        -- SHA-256 of "123456"
        N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92',
        N'123', N'Đường Quang Trung', N'Phường 10', N'Quận Gò Vấp', N'Hồ Chí Minh',
        N'Gold'
    )
) AS v(Id, Username, FirstName, LastName, Gender,
       Email, PhoneNumber, PasswordHash,
       HouseNumber, StreetName, Ward, District, City,
       RankLevel)
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Id = v.Id);

SET IDENTITY_INSERT Users OFF;
GO

PRINT 'Users seeded successfully.';
